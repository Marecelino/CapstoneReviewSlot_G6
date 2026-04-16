using Session.Application.Interfaces;
using Session.Application.Ultils;
using Session.Domain.DTOs;
using Session.Domain.Entities;
using Session.Domain.Enums;
using Session.Infrastructure.Interfaces;

namespace Session.Application.Services
{
    public class ReviewCampaignService : IReviewCampaignService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewCampaignService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewCampaignDto?> CreateReviewCampaignAsync(CreateReviewCampaignDto request)
        {
            var isExisting = await _unitOfWork.ReviewCampaigns
                .ExistsAsync(x =>
                    x.StartTime < request.EndTime &&
                    x.EndTime > request.StartTime
                );
            if (isExisting)
            {
                throw ErrorHelper.BadRequest("Review Campaign is already created!");
            }
            var reviewCampaign = new ReviewCampaign
            {
                Name = request.Name,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = ReviewCampaignStatus.Draft.ToString()
            };

            await _unitOfWork.ReviewCampaigns.AddAsync(reviewCampaign);
            await _unitOfWork.SaveChangesAsync();
            return ToReviewCampaignDto(reviewCampaign);
        }

        public async Task<List<ReviewCampaignDto>> GetAllReviewCampaignsAsync()
        {
            var reviewCampaigns = await _unitOfWork.ReviewCampaigns.GetAllAsync();
            return reviewCampaigns.Select(ToReviewCampaignDto).ToList();
        }

        public async Task<ReviewCampaignDto?> GetReviewCampaignByIdAsync(Guid id)
        {
            var reviewCampaign = await _unitOfWork.ReviewCampaigns.GetByIdAsync(id);
            if (reviewCampaign == null)
            {
                throw ErrorHelper.NotFound("Review Campaign not found!");
            }
            return ToReviewCampaignDto(reviewCampaign);
        }

        public async Task<ReviewCampaignDto?> UpdateReviewCampaignAsync(Guid id, UpdateReviewCampaignDto request)
        {
            var reviewCampaign = await _unitOfWork.ReviewCampaigns.GetByIdAsync(id);
            if (reviewCampaign == null)
            {
                throw ErrorHelper.NotFound("Review Campaign not found!");
            }
            var isExisting = await _unitOfWork.ReviewCampaigns
                .ExistsAsync(x =>
                    x.Id != id &&
                    x.StartTime < request.EndTime &&
                    x.EndTime > request.StartTime
                );
            if (isExisting)
            {
                throw ErrorHelper.BadRequest("Review Campaign is already created!");
            }
            if (!string.IsNullOrEmpty(request.Name) || request.Name != reviewCampaign.Name)
            {
                reviewCampaign.Name = request.Name;
            }
            if (request.StartTime != default(DateTime) || request.StartTime != reviewCampaign.StartTime)
            {
                reviewCampaign.StartTime = request.StartTime;
            }
            if (request.EndTime != default(DateTime) || request.EndTime != reviewCampaign.EndTime)
            {
                reviewCampaign.EndTime = request.EndTime;
            }
            if (!string.IsNullOrEmpty(request.Status) || request.Status.ToString() != reviewCampaign.Status)
            {
                reviewCampaign.Status = request.Status.ToString();
            }
            await _unitOfWork.ReviewCampaigns.Update(reviewCampaign);
            await _unitOfWork.SaveChangesAsync();
            return ToReviewCampaignDto(reviewCampaign);
        }

        public async Task<bool> ChangeReviewCampaignStatusAsync(Guid id, ReviewCampaignStatus status)
        {
            var reviewCampaign = await _unitOfWork.ReviewCampaigns.GetByIdAsync(id);
            if (reviewCampaign == null)
            {
                throw ErrorHelper.NotFound("Review Campaign not found!");
            }
            if (reviewCampaign.Status.Equals(ReviewCampaignStatus.Draft))
            {
                reviewCampaign.Status = ReviewCampaignStatus.Open.ToString();
            }
            else if (reviewCampaign.Status.Equals(ReviewCampaignStatus.Open))
            {
                reviewCampaign.Status = ReviewCampaignStatus.Closed.ToString();
            }
            else if (reviewCampaign.Status.Equals(ReviewCampaignStatus.Closed))
            {
                reviewCampaign.Status = ReviewCampaignStatus.Open.ToString();
            }
            else
            {
                throw ErrorHelper.BadRequest("Invalid Review Campaign status!");
            }
            await _unitOfWork.ReviewCampaigns.Update(reviewCampaign);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReviewCampaignAsync(Guid id)
        {
            var reviewCampaign = await _unitOfWork.ReviewCampaigns.GetByIdAsync(id);
            if (reviewCampaign == null)
            {
                throw ErrorHelper.NotFound("Review Campaign not found!");
            }
            if (reviewCampaign.ReviewSlots != null && reviewCampaign.ReviewSlots.Any())
            {
                throw ErrorHelper.BadRequest("Cannot delete Review Campaign with existing Review Slots!");
            }
            if (reviewCampaign.Status == ReviewCampaignStatus.Open.ToString())
            {
                throw ErrorHelper.BadRequest("Cannot delete active Review Campaign!");
            }
            await _unitOfWork.ReviewCampaigns.SoftRemove(reviewCampaign);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static ReviewCampaignDto ToReviewCampaignDto(ReviewCampaign entity)
        {
            return new ReviewCampaignDto
            {
                Id = entity.Id,
                Name = entity.Name,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = entity.Status,
            };
        }
    }
}
