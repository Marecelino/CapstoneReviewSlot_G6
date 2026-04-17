using Identity.Domain.Enums;

namespace Identity.Application.DTOs;

public record RegisterRequestDto(
    string FullName,
    string Email,
    string Password,
    UserRole Role,
    string? LecturerCode = null,
    string? Department = null);
