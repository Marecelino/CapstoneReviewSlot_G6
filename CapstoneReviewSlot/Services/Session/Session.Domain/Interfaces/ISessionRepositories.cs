using Session.Domain.Entities;

namespace Session.Domain.Interfaces;

public interface IReviewCampaignRepository
{
    Task<ReviewCampaign?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ReviewCampaign>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<ReviewCampaign>> GetOpenAsync(CancellationToken ct = default);
    Task AddAsync(ReviewCampaign campaign, CancellationToken ct = default);
    Task UpdateAsync(ReviewCampaign campaign, CancellationToken ct = default);
}

public interface IReviewSlotRepository
{
    Task<ReviewSlot?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ReviewSlot>> GetByCampaignIdAsync(Guid campaignId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid campaignId, DateOnly date, int slotNumber, CancellationToken ct = default);
    Task AddAsync(ReviewSlot slot, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<ReviewSlot> slots, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    IReviewCampaignRepository Campaigns { get; }
    IReviewSlotRepository Slots { get; }
    ICapstoneGroupRepository CapstoneGroups { get; }
    ICapstoneGroupMemberRepository CapstoneGroupMembers { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface ICapstoneGroupRepository
{
    Task<CapstoneGroup?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CapstoneGroup?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<CapstoneGroup>> GetByCampaignIdAsync(Guid campaignId, CancellationToken ct = default);
    Task<CapstoneGroup?> GetByGroupCodeAsync(string groupCode, CancellationToken ct = default);
    Task AddAsync(CapstoneGroup group, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<CapstoneGroup> groups, CancellationToken ct = default);
    Task UpdateAsync(CapstoneGroup group, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid campaignId, string groupCode, CancellationToken ct = default);
    Task<int> CountByCampaignAsync(Guid campaignId, CancellationToken ct = default);
    Task SoftRemove(CapstoneGroup group, CancellationToken ct = default);
}

public interface ICapstoneGroupMemberRepository
{
    Task<IEnumerable<CapstoneGroupMember>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default);
    Task<CapstoneGroupMember?> GetByMssvAsync(string mssv, CancellationToken ct = default);
    Task AddAsync(CapstoneGroupMember member, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<CapstoneGroupMember> members, CancellationToken ct = default);
}
