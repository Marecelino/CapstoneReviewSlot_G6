using Microsoft.EntityFrameworkCore;
using Session.Domain.Entities;
using Session.Domain.Interfaces;
using Session.Infrastructure.Persistence;

namespace Session.Infrastructure.Repositories;

public class CapstoneGroupRepository : ICapstoneGroupRepository
{
    private readonly SessionDbContext _ctx;
    public CapstoneGroupRepository(SessionDbContext ctx) => _ctx = ctx;

    public async Task<CapstoneGroup?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.CapstoneGroups.FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<CapstoneGroup?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default)
        => await _ctx.CapstoneGroups
                     .Include(g => g.Members)
                     .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IEnumerable<CapstoneGroup>> GetByCampaignIdAsync(Guid campaignId, CancellationToken ct = default)
        => await _ctx.CapstoneGroups
                     .Where(g => g.CampaignId == campaignId)
                     .Include(g => g.Members)
                     .OrderBy(g => g.GroupCode)
                     .ToListAsync(ct);

    public async Task<CapstoneGroup?> GetByGroupCodeAsync(string groupCode, CancellationToken ct = default)
        => await _ctx.CapstoneGroups
                     .Include(g => g.Members)
                     .FirstOrDefaultAsync(g => g.GroupCode == groupCode.ToUpperInvariant(), ct);

    public async Task AddAsync(CapstoneGroup group, CancellationToken ct = default)
        => await _ctx.CapstoneGroups.AddAsync(group, ct);

    public async Task AddRangeAsync(IEnumerable<CapstoneGroup> groups, CancellationToken ct = default)
        => await _ctx.CapstoneGroups.AddRangeAsync(groups, ct);

    public Task UpdateAsync(CapstoneGroup group, CancellationToken ct = default)
    {
        _ctx.CapstoneGroups.Update(group);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid campaignId, string groupCode, CancellationToken ct = default)
        => await _ctx.CapstoneGroups.AnyAsync(
            g => g.CampaignId == campaignId && g.GroupCode == groupCode.ToUpperInvariant(), ct);

    public async Task<int> CountByCampaignAsync(Guid campaignId, CancellationToken ct = default)
        => await _ctx.CapstoneGroups.CountAsync(g => g.CampaignId == campaignId, ct);

    public Task SoftRemove(CapstoneGroup group, CancellationToken ct = default)
    {
        group.IsDeleted = true;
        group.UpdatedAtUtc = DateTime.UtcNow;
        _ctx.CapstoneGroups.Update(group);
        return Task.CompletedTask;
    }
}

public class CapstoneGroupMemberRepository : ICapstoneGroupMemberRepository
{
    private readonly SessionDbContext _ctx;
    public CapstoneGroupMemberRepository(SessionDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<CapstoneGroupMember>> GetByGroupIdAsync(Guid groupId, CancellationToken ct = default)
        => await _ctx.CapstoneGroupMembers.Where(m => m.CapstoneGroupId == groupId).ToListAsync(ct);

    public async Task<CapstoneGroupMember?> GetByMssvAsync(string mssv, CancellationToken ct = default)
        => await _ctx.CapstoneGroupMembers.FirstOrDefaultAsync(m => m.StudentMssv == mssv, ct);

    public async Task AddAsync(CapstoneGroupMember member, CancellationToken ct = default)
        => await _ctx.CapstoneGroupMembers.AddAsync(member, ct);

    public async Task AddRangeAsync(IEnumerable<CapstoneGroupMember> members, CancellationToken ct = default)
        => await _ctx.CapstoneGroupMembers.AddRangeAsync(members, ct);
}
