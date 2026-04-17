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
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
