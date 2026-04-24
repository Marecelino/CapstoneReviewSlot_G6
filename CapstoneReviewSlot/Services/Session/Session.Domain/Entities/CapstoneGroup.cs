using Entities;

namespace Session.Domain.Entities;

/// <summary>
/// Nhóm sinh viên capstone. Mỗi nhóm có nhiều thành viên (CapstoneGroupMember).
/// MentorLecturerId là ID của GVHD - được dùng để enforce business rule
/// "GV không thể review nhóm mình hướng dẫn".
/// </summary>
public class CapstoneGroup : BaseEntity
{
    public Guid CampaignId { get; set; }
    public string GroupCode { get; set; } = default!;       // e.g., "GFA25SE01"
    public string ProjectCode { get; set; } = default!;    // e.g., "FA25SE135"
    public string ProjectNameEn { get; set; } = default!;
    public string ProjectNameVn { get; set; } = default!;

    /// <summary>GVHD chính - LecturerId (Guid) từ Identity service.</summary>
    public Guid MentorLecturerId { get; set; }

    /// <summary>Danh sách supervisors (GVHD phụ, nếu có). Lưu dưới dạng JSON array.</summary>
    public string SupervisorJson { get; set; } = "[]";

    // Navigation
    public ReviewCampaign? ReviewCampaign { get; set; }
    public ICollection<CapstoneGroupMember> Members { get; set; } = new List<CapstoneGroupMember>();

    public CapstoneGroup() { }

    public static CapstoneGroup Create(
        Guid campaignId,
        string groupCode,
        string projectCode,
        string projectNameEn,
        string projectNameVn,
        Guid mentorLecturerId,
        List<Guid>? additionalSupervisors = null)
    {
        var supervisors = additionalSupervisors ?? new List<Guid>();
        return new CapstoneGroup
        {
            CampaignId = campaignId,
            GroupCode = groupCode.Trim().ToUpperInvariant(),
            ProjectCode = projectCode.Trim(),
            ProjectNameEn = projectNameEn.Trim(),
            ProjectNameVn = projectNameVn.Trim(),
            MentorLecturerId = mentorLecturerId,
            SupervisorJson = System.Text.Json.JsonSerializer.Serialize(supervisors)
        };
    }

    public List<Guid> GetAdditionalSupervisorIds()
    {
        if (string.IsNullOrWhiteSpace(SupervisorJson))
            return new List<Guid>();
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(SupervisorJson) ?? new List<Guid>();
        }
        catch
        {
            return new List<Guid>();
        }
    }
}
