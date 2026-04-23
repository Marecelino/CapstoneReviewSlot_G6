using Entities;

namespace Session.Domain.Entities;

/// <summary>
/// Thành viên trong nhóm capstone. Mỗi sinh viên thuộc 1 nhóm.
/// </summary>
public class CapstoneGroupMember : BaseEntity
{
    public Guid CapstoneGroupId { get; set; }
    public string StudentMssv { get; set; } = default!;  // e.g., "SE173501"
    public string StudentName { get; set; } = default!;
    public string Department { get; set; } = default!;    // e.g., "BIT", "BSE", "CF", "ITS"

    // Navigation
    public CapstoneGroup? CapstoneGroup { get; set; }

    public CapstoneGroupMember() { }

    public static CapstoneGroupMember Create(
        Guid capstoneGroupId,
        string studentMssv,
        string studentName,
        string department)
    {
        return new CapstoneGroupMember
        {
            CapstoneGroupId = capstoneGroupId,
            StudentMssv = studentMssv.Trim(),
            StudentName = studentName.Trim(),
            Department = department.Trim().ToUpperInvariant()
        };
    }
}
