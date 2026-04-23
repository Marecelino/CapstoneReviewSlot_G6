using Session.Domain.DTOs;

namespace Session.Application.Interfaces;

public interface ICapstoneGroupService
{
    Task<CapstoneGroupDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CapstoneGroupDto?> GetByGroupCodeAsync(string groupCode, CancellationToken ct = default);
    Task<List<CapstoneGroupDto>> GetByCampaignAsync(Guid campaignId, CancellationToken ct = default);
    Task<CapstoneGroupDto> CreateAsync(CreateCapstoneGroupDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> CountByCampaignAsync(Guid campaignId, CancellationToken ct = default);
}
