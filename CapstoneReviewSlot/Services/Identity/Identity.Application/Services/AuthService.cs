using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Abstractions.Services;
using Identity.Application.DTOs.Auth;
using Identity.Domain.Constants;
using Identity.Domain.Entities;

namespace Identity.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILecturerRepository _lecturerRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUserRepository userRepository,
        ILecturerRepository lecturerRepository,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService)
    {
        _userRepository     = userRepository;
        _lecturerRepository = lecturerRepository;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService    = jwtTokenService;
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
            "", // Hash will be set below
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

        // Nếu user là Lecturer → load Lecturer record để lấy LecturerId cho JWT
        Lecturer? lecturer = null;
        if (user.Role == Identity.Domain.Enums.UserRole.Lecturer)
        {
            lecturer = await _lecturerRepository.GetByUserIdAsync(user.Id, cancellationToken);
            // Nếu thiếu bản ghi Lecturer thì là dữ liệu không nhất quán
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

        // Kiểm tra email trùng
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException("Email đã tồn tại trong hệ thống.");

        // Kiểm tra LecturerCode trùng
        var codeExists = await _lecturerRepository.ExistsByCodeAsync(request.LecturerCode, cancellationToken);
        if (codeExists)
            throw new InvalidOperationException($"Mã giảng viên '{request.LecturerCode}' đã được sử dụng.");

        // 1. Tạo User
        var user = User.Create(
            request.FullName.Trim(),
            normalizedEmail,
            "", // Hash will be set below
            Identity.Domain.Enums.UserRole.Lecturer);

        user.PasswordHash = _passwordHasherService.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken); // flush để lấy user.Id

        // 2. Tạo Lecturer record — dùng user.Id (Guid) vừa được gán
        var lecturer = Lecturer.Create(user.Id, request.LecturerCode, request.Department);
        await _lecturerRepository.AddAsync(lecturer, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken); // flush Lecturer (cùng DbContext)

        return BuildResponse(user, lecturer);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────
    private AuthResponse BuildResponse(User user, Lecturer? lecturer)
    {
        return new AuthResponse
        {
            UserId     = user.Id,
            LecturerId = lecturer?.LecturerId,
            Email      = user.Email,
            FullName   = user.FullName,
            Role       = user.Role.ToString(),
            Token      = _jwtTokenService.GenerateToken(user, lecturer?.LecturerId)
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}