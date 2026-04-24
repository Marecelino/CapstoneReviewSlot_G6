using Session.Application.Interfaces;
using Session.Application.Ultils;
using Session.Domain.DTOs;
using Session.Domain.Entities;
using Session.Infrastructure.Interfaces;

namespace Session.Application.Services
{
    public class ReviewSlotService : IReviewSlotService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewSlotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewSlotDto?> GetReviewSlotByIdAsync(Guid id)
        {
            var reviewSlot = await _unitOfWork.ReviewSlots.GetByIdAsync(id);
            if (reviewSlot == null)
            {
                throw ErrorHelper.NotFound("Review Slot not found!");
            }
            return ToReviewSlotDto(reviewSlot);
        }

        public async Task<List<ReviewSlotDto>> GetReviewSlotsByDate(DateOnly date)
        {
            var reviewSlots = await _unitOfWork.ReviewSlots
                .FindAsync(rs => rs.ReviewDate == date);

            return reviewSlots.Select(ToReviewSlotDto).ToList();
        }

        public async Task<ReviewSlotDto?> UpdateReviewSlotRoom(Guid id, string newRoom)
        {
            var reviewSlot = await _unitOfWork.ReviewSlots.GetByIdAsync(id);
            if (reviewSlot == null)
            {
                throw ErrorHelper.NotFound("Review Slot not found!");
            }
            reviewSlot.Room = newRoom;
            await _unitOfWork.SaveChangesAsync();
            return ToReviewSlotDto(reviewSlot);
        }

        public async Task<bool> DeleteReviewSlotAsync(Guid id)
        {
            var reviewSlot = await _unitOfWork.ReviewSlots.GetByIdAsync(id);
            if (reviewSlot == null)
            {
                throw ErrorHelper.NotFound("Review Slot not found!");
            }
            await _unitOfWork.ReviewSlots.SoftRemove(reviewSlot);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static ReviewSlotDto ToReviewSlotDto(ReviewSlot reviewSlot)
        {
            return new ReviewSlotDto(
                reviewSlot.Id,
                reviewSlot.CampaignId,
                reviewSlot.ReviewDate,
                reviewSlot.SlotNumber,
                reviewSlot.StartTime,
                reviewSlot.EndTime,
                reviewSlot.Room,
                reviewSlot.MaxCapacity);
        }
    }
}
