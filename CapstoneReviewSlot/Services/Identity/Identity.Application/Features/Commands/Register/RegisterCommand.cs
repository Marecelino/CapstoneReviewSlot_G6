using Identity.Application.DTOs;
using Identity.Application.Interfaces;
using Identity.Application.Abstractions.Security;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;
using MediatR;

namespace Identity.Application.Features.Commands.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    string? LecturerCode = null,
    string? Department = null) : IRequest<UserDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _tokenService;

    public RegisterCommandHandler(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenService tokenService)
    {
        _uow = uow;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _uow.Users.ExistsByEmailAsync(request.Email, cancellationToken))
            throw new InvalidOperationException($"Email '{request.Email}' đã được đăng ký.");

        if (request.Role == UserRole.Lecturer)
        {
            if (string.IsNullOrWhiteSpace(request.LecturerCode))
                throw new InvalidOperationException("LecturerCode bắt buộc đối với giảng viên.");

            if (await _uow.Lecturers.ExistsByCodeAsync(request.LecturerCode, cancellationToken))
                throw new InvalidOperationException($"LecturerCode '{request.LecturerCode}' đã tồn tại.");
        }

        var user = User.Create(request.FullName, request.Email, _hasher.Hash(request.Password), request.Role);
        await _uow.Users.AddAsync(user, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken); // Lưu để có UserId

        LecturerDto? lecturerDto = null;
        if (request.Role == UserRole.Lecturer && !string.IsNullOrWhiteSpace(request.LecturerCode))
        {
            var lecturer = Lecturer.Create(user.Id, request.LecturerCode, request.Department);
            await _uow.Lecturers.AddAsync(lecturer, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            lecturerDto = new LecturerDto(lecturer.LecturerId, lecturer.LecturerCode, lecturer.Department);
        }

        return new UserDto(user.Id, user.Email, user.FullName, user.Role, user.Status, lecturerDto);
    }
}
