using MediatR;
using Session.Application.DTOs;
using Session.Domain.Interfaces;

namespace Session.Application.Features.Queries.GetSlotsByCampaign;

public record GetSlotsByCampaignQuery(Guid CampaignId) : IRequest<IEnumerable<ReviewSlotDto>>;

public class GetSlotsByCampaignQueryHandler
    : IRequestHandler<GetSlotsByCampaignQuery, IEnumerable<ReviewSlotDto>>
{
    private readonly IUnitOfWork _uow;
    public GetSlotsByCampaignQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ReviewSlotDto>> Handle(
        GetSlotsByCampaignQuery request, CancellationToken ct)
    {
        // Verify campaign exists
        var campaign = await _uow.Campaigns.GetByIdAsync(request.CampaignId, ct)
            ?? throw new KeyNotFoundException($"Campaign {request.CampaignId} không tồn tại.");

        var slots = await _uow.Slots.GetByCampaignIdAsync(request.CampaignId, ct);
        return slots.Select(s => new ReviewSlotDto(
            s.Id, s.CampaignId, s.ReviewDate, s.SlotNumber,
            s.StartTime, s.EndTime, s.Room, s.MaxCapacity));
    }
}
