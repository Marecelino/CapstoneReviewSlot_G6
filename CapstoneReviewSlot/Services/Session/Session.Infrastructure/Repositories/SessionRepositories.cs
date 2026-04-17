using Microsoft.EntityFrameworkCore;
using Session.Domain.Entities;
using Session.Domain.Enums;
using Session.Domain.Interfaces;
using Session.Infrastructure.Persistence;

namespace Session.Infrastructure.Repositories;

public class ReviewCampaignRepository : IReviewCampaignRepository
{
    private readonly SessionDbContext _ctx;
    public ReviewCampaignRepository(SessionDbContext ctx) => _ctx = ctx;

    public async Task<ReviewCampaign?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.ReviewCampaigns.Include(c => c.ReviewSlots)
                                     .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IEnumerable<ReviewCampaign>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.ReviewCampaigns.ToListAsync(ct);

    public async Task<IEnumerable<ReviewCampaign>> GetOpenAsync(CancellationToken ct = default)
        => await _ctx.ReviewCampaigns
                     .Where(c => c.Status == CampaignStatus.Open.ToString())
                     .ToListAsync(ct);

    public async Task AddAsync(ReviewCampaign campaign, CancellationToken ct = default)
        => await _ctx.ReviewCampaigns.AddAsync(campaign, ct);

    public Task UpdateAsync(ReviewCampaign campaign, CancellationToken ct = default)
    {
        _ctx.ReviewCampaigns.Update(campaign);
        return Task.CompletedTask;
    }
}

public class ReviewSlotRepository : IReviewSlotRepository
{
    private readonly SessionDbContext _ctx;
    public ReviewSlotRepository(SessionDbContext ctx) => _ctx = ctx;

    public async Task<ReviewSlot?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.ReviewSlots.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IEnumerable<ReviewSlot>> GetByCampaignIdAsync(Guid campaignId, CancellationToken ct = default)
        => await _ctx.ReviewSlots
                     .Where(s => s.CampaignId == campaignId)
                     .OrderBy(s => s.ReviewDate)
                     .ThenBy(s => s.SlotNumber)
                     .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid campaignId, DateOnly date, int slotNumber, CancellationToken ct = default)
        => await _ctx.ReviewSlots.AnyAsync(
            s => s.CampaignId == campaignId && s.ReviewDate == date && s.SlotNumber == slotNumber, ct);

    public async Task AddAsync(ReviewSlot slot, CancellationToken ct = default)
        => await _ctx.ReviewSlots.AddAsync(slot, ct);

    public async Task AddRangeAsync(IEnumerable<ReviewSlot> slots, CancellationToken ct = default)
        => await _ctx.ReviewSlots.AddRangeAsync(slots, ct);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly SessionDbContext _ctx;
    private ReviewCampaignRepository? _campaigns;
    private ReviewSlotRepository? _slots;

    public UnitOfWork(SessionDbContext ctx) => _ctx = ctx;

    public IReviewCampaignRepository Campaigns => _campaigns ??= new ReviewCampaignRepository(_ctx);
    public IReviewSlotRepository Slots => _slots ??= new ReviewSlotRepository(_ctx);
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _ctx.SaveChangesAsync(ct);
}
