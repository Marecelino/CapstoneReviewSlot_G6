using Identity.Domain.Enums;

namespace Identity.Application.DTOs;

public record UserDto(
    Guid UserId,
    string Email,
    string FullName,
    UserRole Role,
    UserStatus Status,
    LecturerDto? Lecturer = null);

public record LecturerDto(
    Guid LecturerId,
    string LecturerCode,
    string? Department);
