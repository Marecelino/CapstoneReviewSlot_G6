using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Security;

public interface IJwtTokenService
{
    string GenerateToken(User user, Guid? lecturerId = null);
    bool ValidateToken(string token);
    Guid? GetUserIdFromToken(string token);
    Guid? GetLecturerIdFromToken(string token);
}
