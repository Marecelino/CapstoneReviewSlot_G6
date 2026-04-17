namespace Identity.Application.DTOs.Auth;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public Guid? LecturerId { get; set; }       // null nếu không phải Lecturer
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string Token { get; set; } = default!;
}
