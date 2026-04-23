using System.Security.Cryptography;
using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Abstractions.Services;
using Identity.Application.DTOs.Auth;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILecturerRepository _lecturerRepository;
    private readonly IPasswordResetOtpRepository _passwordResetOtpRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        ILecturerRepository lecturerRepository,
        IPasswordResetOtpRepository passwordResetOtpRepository,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _lecturerRepository = lecturerRepository;
        _passwordResetOtpRepository = passwordResetOtpRepository;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
        _emailService = emailService;
    }

    // ─── Register (Student tự đăng ký) ──────────────────────────────────────
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Email đã tồn tại trong hệ thống.");

        var user = User.Create(
            request.FullName.Trim(),
            normalizedEmail,
            "",
            Identity.Domain.Enums.UserRole.Student);

        user.PasswordHash = _passwordHasherService.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BuildResponse(user, lecturer: null);
    }

    // ─── Login ───────────────────────────────────────────────────────────────
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        var isValid = _passwordHasherService.VerifyPassword(user, request.Password);
        if (!isValid)
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        Lecturer? lecturer = null;
        if (user.Role == Identity.Domain.Enums.UserRole.Lecturer)
        {
            lecturer = await _lecturerRepository.GetByUserIdAsync(user.Id, cancellationToken);
            if (lecturer is null)
                throw new InvalidOperationException(
                    $"Tài khoản Giảng viên (email: {user.Email}) không có thông tin Lecturer tương ứng. Vui lòng liên hệ Admin.");
        }

        return BuildResponse(user, lecturer);
    }

    // ─── Admin tạo tài khoản Giảng viên ──────────────────────────────────────
    public async Task<AuthResponse> CreateLecturerAccountAsync(
        CreateLecturerAccountRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Email đã tồn tại trong hệ thống.");

        var codeExists = await _lecturerRepository.ExistsByCodeAsync(request.LecturerCode, cancellationToken);
        if (codeExists)
            throw new InvalidOperationException($"Mã giảng viên '{request.LecturerCode}' đã được sử dụng.");

        var user = User.Create(
            request.FullName.Trim(),
            normalizedEmail,
            "",
            Identity.Domain.Enums.UserRole.Lecturer);

        user.PasswordHash = _passwordHasherService.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var lecturer = Lecturer.Create(user.Id, request.LecturerCode, request.Department);
        await _lecturerRepository.AddAsync(lecturer, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return BuildResponse(user, lecturer);
    }

    // ─── Quên mật khẩu: gửi OTP qua email ───────────────────────────────────
    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        // Không leak việc email có tồn tại hay không
        if (user is null)
            return;

        var otp = GenerateOtp();

        var passwordResetOtp = new PasswordResetOtp
        {
            UserId = user.Id,
            Email = normalizedEmail,
            OtpHash = _passwordHasherService.HashPassword(user, otp),
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10)
        };

        await _passwordResetOtpRepository.AddAsync(passwordResetOtp, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var htmlBody = $"""
            <p>Xin chào {user.FullName},</p>
            <p>Mã OTP để đặt lại mật khẩu của bạn là:</p>
            <h2>{otp}</h2>
            <p>Mã này sẽ hết hạn sau 10 phút.</p>
            <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
            """;

        await _emailService.SendAsync(
            normalizedEmail,
            "OTP đặt lại mật khẩu",
            htmlBody,
            cancellationToken);
    }

    // ─── Quên mật khẩu: xác thực OTP ────────────────────────────────────────
    public async Task VerifyForgotPasswordOtpAsync(
        VerifyForgotPasswordOtpRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        var latestOtp = await _passwordResetOtpRepository.GetLatestByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || latestOtp is null || latestOtp.UsedAtUtc.HasValue || latestOtp.ExpiresAtUtc < DateTime.UtcNow)
            throw new InvalidOperationException("OTP không hợp lệ hoặc đã hết hạn.");

        var otpUser = User.Create(user.FullName, user.Email, latestOtp.OtpHash, user.Role);

        var isValidOtp = _passwordHasherService.VerifyPassword(otpUser, request.Otp);
        if (!isValidOtp)
            throw new InvalidOperationException("OTP không chính xác.");

        latestOtp.VerifiedAtUtc = DateTime.UtcNow;
        _passwordResetOtpRepository.Update(latestOtp);

        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    // ─── Quên mật khẩu: đặt mật khẩu mới ─────────────────────────────────────
    public async Task ResetPasswordAsync(
        ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(request.Email);

        var user = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        var latestOtp = await _passwordResetOtpRepository.GetLatestByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || latestOtp is null || latestOtp.UsedAtUtc.HasValue || latestOtp.ExpiresAtUtc < DateTime.UtcNow)
            throw new InvalidOperationException("OTP không hợp lệ hoặc đã hết hạn.");

        if (!latestOtp.VerifiedAtUtc.HasValue)
            throw new InvalidOperationException("OTP chưa được xác thực.");

        var otpUser = User.Create(user.FullName, user.Email, latestOtp.OtpHash, user.Role);

        var isValidOtp = _passwordHasherService.VerifyPassword(otpUser, request.Otp);
        if (!isValidOtp)
            throw new InvalidOperationException("OTP không chính xác.");

        user.PasswordHash = _passwordHasherService.HashPassword(user, request.NewPassword);
        latestOtp.UsedAtUtc = DateTime.UtcNow;

        _passwordResetOtpRepository.Update(latestOtp);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    // ─── Đổi mật khẩu khi đã đăng nhập ───────────────────────────────────────
    public async Task ChangePasswordAsync(
    Guid userId,
    ChangePasswordRequest request,
    CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            throw new InvalidOperationException("Không tìm thấy người dùng.");

        var isOldPasswordValid = _passwordHasherService.VerifyPassword(user, request.OldPassword);
        if (!isOldPasswordValid)
            throw new InvalidOperationException("Mật khẩu cũ không đúng.");

        if (request.NewPassword != request.ConfirmNewPassword)
            throw new InvalidOperationException("Xác nhận mật khẩu mới không khớp.");

        if (request.OldPassword == request.NewPassword)
            throw new InvalidOperationException("Mật khẩu mới không được trùng mật khẩu cũ.");

        user.PasswordHash = _passwordHasherService.HashPassword(user, request.NewPassword);

        await _userRepository.SaveChangesAsync(cancellationToken);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────
    private AuthResponse BuildResponse(User user, Lecturer? lecturer)
    {
        return new AuthResponse
        {
            UserId = user.Id,
            LecturerId = lecturer?.LecturerId,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            Token = _jwtTokenService.GenerateToken(user, lecturer?.LecturerId)
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string GenerateOtp()
    {
        return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
    }
}