using Entities;

namespace Registration.Domain.Entities;

/// <summary>
/// Student slot preference - allows students to register their preferred review slots
/// before the manager performs auto-assignment.
/// </summary>
public class StudentSlotPreference : BaseEntity
{
    public Guid CapstoneGroupId { get; private set; }
    public Guid ReviewSlotId { get; private set; }
    public int PreferenceOrder { get; private set; }  // 1 = first choice, 2 = second choice, etc.
    public string StudentMssv { get; private set; } = default!;

    // Navigation
    public StudentSlotPreference() { }

    public static StudentSlotPreference Create(
        Guid capstoneGroupId,
        Guid reviewSlotId,
        int preferenceOrder,
        string studentMssv)
    {
        return new StudentSlotPreference
        {
            CapstoneGroupId = capstoneGroupId,
            ReviewSlotId = reviewSlotId,
            PreferenceOrder = preferenceOrder,
            StudentMssv = studentMssv
        };
    }
}
