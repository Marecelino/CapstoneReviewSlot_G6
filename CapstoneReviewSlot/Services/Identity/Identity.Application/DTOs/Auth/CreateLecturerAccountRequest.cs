namespace Identity.Application.DTOs.Auth;

public class CreateLecturerAccountRequest
{
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Password { get; set; } = default!;

    /// <summary>Mã giảng viên, ví dụ: GV001</summary>
    public string LecturerCode { get; set; } = default!;

    /// <summary>Khoa/Bộ môn, ví dụ: "Công nghệ thông tin"</summary>
    public string? Department { get; set; }
}
