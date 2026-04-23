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
                Status = ReviewCampaignStatus.Draft.ToString(),
                ReviewSlots = new List<ReviewSlot>()
            };

            await _unitOfWork.ReviewCampaigns.AddAsync(reviewCampaign);
            await _unitOfWork.SaveChangesAsync();

            await GenerateDefaultSlotsAsync(reviewCampaign);

            await _unitOfWork.SaveChangesAsync();
            return ToReviewCampaignDto(reviewCampaign);
        }

        public async Task<List<ReviewCampaignDto>> GetAllReviewCampaignsAsync()
        {
            var reviewCampaigns = await _unitOfWork.ReviewCampaigns.GetAllAsync();
            var reviewSlots = await _unitOfWork.ReviewSlots.GetAllAsync();

            return reviewCampaigns.Select(campaign =>
            {
                var dto = ToReviewCampaignDto(campaign);

                dto.ReviewSlots = reviewSlots
                    .Where(slot => slot.CampaignId == campaign.Id)
                    .Select(ToReviewSlotDto)
                    .ToList();

                return dto;
            }).ToList();
        }

        public async Task<ReviewCampaignDto?> GetReviewCampaignByIdAsync(Guid id)
        {
            var reviewCampaign = await _unitOfWork.ReviewCampaigns.GetByIdAsync(id);
            if (reviewCampaign == null)
            {
                throw ErrorHelper.NotFound("Review Campaign not found!");
            }

            var reviewSlots = await _unitOfWork.ReviewSlots
                .FindAsync(x => x.CampaignId == id);

            var dto = ToReviewCampaignDto(reviewCampaign);
            dto.ReviewSlots = reviewSlots.Select(ToReviewSlotDto).ToList();

            return dto;
        }

        public async Task<ReviewCampaignDto?> UpdateReviewCampaignAsync(Guid id, UpdateReviewCampaignDto request)
        {
            var reviewCampaign = await _unitOfWork.ReviewCampaigns.GetByIdAsync(id);
            if (reviewCampaign == null)
            {
                throw ErrorHelper.NotFound("Review Campaign not found!");
            }

            // check overlap
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

            // detect time change
            var isTimeChanged =
                (request.StartTime != default && request.StartTime != reviewCampaign.StartTime) ||
                (request.EndTime != default && request.EndTime != reviewCampaign.EndTime);

            // update fields
            if (!string.IsNullOrEmpty(request.Name) && request.Name != reviewCampaign.Name)
            {
                reviewCampaign.Name = request.Name;
            }

            if (request.StartTime != default && request.StartTime != reviewCampaign.StartTime)
            {
                reviewCampaign.StartTime = request.StartTime;
            }

            if (request.EndTime != default && request.EndTime != reviewCampaign.EndTime)
            {
                reviewCampaign.EndTime = request.EndTime;
            }

            if (isTimeChanged)
            {
                var oldSlots = await _unitOfWork.ReviewSlots
                    .FindAsync(x => x.CampaignId == id);

                if (oldSlots.Any())
                {
                    await _unitOfWork.ReviewSlots.HardRemoveRange(oldSlots);
                    await _unitOfWork.SaveChangesAsync();
                }

                await GenerateDefaultSlotsAsync(reviewCampaign);
            }

            await _unitOfWork.ReviewCampaigns.Update(reviewCampaign);
            await _unitOfWork.SaveChangesAsync();

            var updatedCampaign = await _unitOfWork.ReviewCampaigns
            .GetByIdAsync(id); // include slots

            return ToReviewCampaignDto(updatedCampaign);
        }

        public async Task<bool> ChangeReviewCampaignStatusAsync(Guid id, ReviewCampaignStatus status)
        {
            var reviewCampaign = await _unitOfWork.ReviewCampaigns.GetByIdAsync(id);
            if (reviewCampaign == null)
            {
                throw ErrorHelper.NotFound("Review Campaign not found!");
            }
            reviewCampaign.Status = status.ToString();
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
            //if (reviewCampaign.ReviewSlots != null && reviewCampaign.ReviewSlots.Any())
            //{
            //    throw ErrorHelper.BadRequest("Cannot delete Review Campaign with existing Review Slots!");
            //}
            if (reviewCampaign.Status == ReviewCampaignStatus.Open.ToString())
            {
                throw ErrorHelper.BadRequest("Cannot delete active Review Campaign!");
            }
            var oldSlots = await _unitOfWork.ReviewSlots
                    .FindAsync(x => x.CampaignId == id);

            if (oldSlots.Any())
            {
                await _unitOfWork.ReviewSlots.HardRemoveRange(oldSlots);
                await _unitOfWork.SaveChangesAsync();
            }
            await _unitOfWork.ReviewCampaigns.SoftRemove(reviewCampaign);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task GenerateDefaultSlotsAsync(ReviewCampaign campaign)
        {
            var slots = new List<ReviewSlot>();

            var startDate = DateOnly.FromDateTime(campaign.StartTime);
            var endDate = DateOnly.FromDateTime(campaign.EndTime);

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dayOfWeek = date.DayOfWeek;
                if (dayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }

                foreach (var slot in DefaultSlots)
                {
                    slots.Add(new ReviewSlot
                    {
                        Id = Guid.NewGuid(),
                        CampaignId = campaign.Id,
                        ReviewDate = date,
                        SlotNumber = slot.SlotNumber,
                        StartTime = slot.Start,
                        EndTime = slot.End,
                        Room = string.Empty,
                        MaxCapacity = 30
                    });
                }
            }

            await _unitOfWork.ReviewSlots.AddRangeAsync(slots);
            await _unitOfWork.SaveChangesAsync();
        }

        private static readonly List<(int SlotNumber, TimeOnly Start, TimeOnly End)> DefaultSlots =
        [
            (1, new TimeOnly(7, 0), new TimeOnly(9, 15)),
            (2, new TimeOnly(9, 30), new TimeOnly(11, 45)),
            (3, new TimeOnly(12, 30), new TimeOnly(14, 45)),
            (4, new TimeOnly(15, 0), new TimeOnly(17, 15))
        ];

        private static ReviewCampaignDto ToReviewCampaignDto(ReviewCampaign entity)
        {
            return new ReviewCampaignDto
            {
                Id = entity.Id,
                Name = entity.Name,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = entity.Status,
                ReviewSlots = entity.ReviewSlots?.Select(slot => new ReviewSlotDto
                {
                    Id = slot.Id,
                    CampaignId = slot.CampaignId,
                    ReviewDate = slot.ReviewDate,
                    SlotNumber = slot.SlotNumber,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Room = slot.Room,
                    MaxCapacity = slot.MaxCapacity
                }).ToList() ?? new List<ReviewSlotDto>()
            };
        }

        private ReviewSlotDto ToReviewSlotDto(ReviewSlot reviewSlot)
        {
            return new ReviewSlotDto
            {
                Id = reviewSlot.Id,
                CampaignId = reviewSlot.CampaignId,
                ReviewDate = reviewSlot.ReviewDate,
                SlotNumber = reviewSlot.SlotNumber,
                StartTime = reviewSlot.StartTime,
                EndTime = reviewSlot.EndTime,
                Room = reviewSlot.Room,
                MaxCapacity = reviewSlot.MaxCapacity
            };
        }
    }
}
