namespace Identity.Domain.Entities;

/// <summary>
/// Bảng Lecturer — mỗi User có Role = "Lecturer" sẽ có 1 Lecturer record.
/// LecturerId (int, IDENTITY) là ID được dùng ở AvailabilityDB / SessionDB để tham chiếu logic.
/// </summary>
public class Lecturer
{
    public Guid LecturerId { get; private set; }      // PK (Guid) — được dùng bởi các service khác
    public Guid UserId { get; private set; }           // FK → User.Id (Guid)
    public string LecturerCode { get; private set; } = default!;
    public string? Department { get; private set; }

    // Navigation
    public User User { get; private set; } = default!;

    private Lecturer() { }

    public static Lecturer Create(Guid userId, string lecturerCode, string? department = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId must not be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(lecturerCode))
            throw new ArgumentException("LecturerCode is required.", nameof(lecturerCode));

        return new Lecturer
        {
            UserId      = userId,
            LecturerCode = lecturerCode.Trim().ToUpperInvariant(),
            Department  = department?.Trim()
        };
    }

    public void UpdateDepartment(string? department)
    {
        Department = department?.Trim();
    }
}
