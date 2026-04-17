using Entities;
using Identity.Domain.Enums;

namespace Identity.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public UserRole Role { get; set; } = UserRole.Student;
    public UserStatus Status { get; set; } = UserStatus.Active;

    // Navigation: chỉ có khi Role = "Lecturer"
    public Lecturer? Lecturer { get; set; }

    private User() { }

    public static User Create(string fullName, string email, string passwordHash, UserRole role)
    {
        return new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            Status = UserStatus.Active
        };
    }
}
