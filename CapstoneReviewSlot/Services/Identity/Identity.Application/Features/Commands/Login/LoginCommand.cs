using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Application.Abstractions.Security;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Features.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _tokenService;

    public LoginCommandHandler(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenService tokenService)
    {
        _uow = uow;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _uow.Users.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");

        if (user.Status == Domain.Enums.UserStatus.Inactive)
            throw new UnauthorizedAccessException("Tài khoản đã bị vô hiệu hóa.");

        // Lấy LecturerId nếu là Lecturer (embed vào JWT để Availability Service dùng)
        Guid? lecturerId = null;
        if (user.Role == Domain.Enums.UserRole.Lecturer)
        {
            var lecturer = await _uow.Lecturers.GetByUserIdAsync(user.Id, cancellationToken);
            lecturerId = lecturer?.LecturerId;
        }

        var lecturerEntity = lecturerId.HasValue
            ? await _uow.Lecturers.GetByUserIdAsync(user.Id, cancellationToken)
            : null;

        var token = _tokenService.GenerateToken(user, lecturerEntity?.LecturerId);

        var lecturerDto = lecturerEntity != null
            ? new LecturerDto(lecturerEntity.LecturerId, lecturerEntity.LecturerCode, lecturerEntity.Department)
            : null;

        var userDto = new UserDto(user.Id, user.Email, user.FullName, user.Role, user.Status, lecturerDto);
        return new LoginResponseDto(token, "Bearer", 3600, userDto);
    }
}
