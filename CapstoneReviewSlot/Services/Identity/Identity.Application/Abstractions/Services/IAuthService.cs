using Identity.Application.DTOs.Auth;

namespace Identity.Application.Abstractions.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> CreateLecturerAccountAsync(CreateLecturerAccountRequest request, CancellationToken cancellationToken = default);
}
