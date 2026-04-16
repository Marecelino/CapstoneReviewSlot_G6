using Session.Domain.DTOs;
using Session.Domain.Enums;

namespace Session.Application.Interfaces
{
    public interface IReviewCampaignService
    {
        Task<ReviewCampaignDto?> CreateReviewCampaignAsync(CreateReviewCampaignDto request);
        Task<List<ReviewCampaignDto>> GetAllReviewCampaignsAsync();
        Task<ReviewCampaignDto?> GetReviewCampaignByIdAsync(Guid id);
        Task<ReviewCampaignDto?> UpdateReviewCampaignAsync(Guid id, UpdateReviewCampaignDto request);
        Task<bool> ChangeReviewCampaignStatusAsync(Guid id, ReviewCampaignStatus status);
        Task<bool> DeleteReviewCampaignAsync(Guid id);
    }
}
