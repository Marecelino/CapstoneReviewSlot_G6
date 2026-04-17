using MediatR;
using Session.Application.DTOs;
using Session.Domain.Entities;
using Session.Domain.Enums;
using Session.Domain.Interfaces;

namespace Session.Application.Features.Commands.CreateCampaign;

public record CreateCampaignCommand(
    string Name,
    DateTime StartTime,
    DateTime EndTime,
    int MaxGroupsPerLecturer,
    int RequiredReviewersPerGroup) : IRequest<ReviewCampaignDto>;

public class CreateCampaignCommandHandler
    : IRequestHandler<CreateCampaignCommand, ReviewCampaignDto>
{
    private readonly IUnitOfWork _uow;
    public CreateCampaignCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReviewCampaignDto> Handle(CreateCampaignCommand request, CancellationToken ct)
    {
        var campaign = ReviewCampaign.Create(
            request.Name, request.StartTime, request.EndTime,
            request.MaxGroupsPerLecturer, request.RequiredReviewersPerGroup);

        await _uow.Campaigns.AddAsync(campaign, ct);
        await _uow.SaveChangesAsync(ct);

        return new ReviewCampaignDto(
            campaign.Id, campaign.Name, campaign.StartTime, campaign.EndTime,
            campaign.MaxGroupsPerLecturer, campaign.RequiredReviewersPerGroup, campaign.Status);
    }
}
