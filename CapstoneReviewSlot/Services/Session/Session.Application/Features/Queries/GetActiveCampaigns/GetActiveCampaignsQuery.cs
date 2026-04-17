using MediatR;
using Session.Application.DTOs;
using Session.Domain.Interfaces;

namespace Session.Application.Features.Queries.GetActiveCampaigns;

public record GetActiveCampaignsQuery : IRequest<IEnumerable<ReviewCampaignDto>>;

public class GetActiveCampaignsQueryHandler
    : IRequestHandler<GetActiveCampaignsQuery, IEnumerable<ReviewCampaignDto>>
{
    private readonly IUnitOfWork _uow;
    public GetActiveCampaignsQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ReviewCampaignDto>> Handle(
        GetActiveCampaignsQuery request, CancellationToken ct)
    {
        var campaigns = await _uow.Campaigns.GetOpenAsync(ct);
        return campaigns.Select(c => new ReviewCampaignDto(
            c.Id, c.Name, c.StartTime, c.EndTime,
            c.MaxGroupsPerLecturer, c.RequiredReviewersPerGroup, c.Status));
    }
}
