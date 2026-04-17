using Session.Domain.DTOs;

namespace Session.Application.Interfaces
{
    public interface IReviewSlotService
    {
        Task<ReviewSlotDto?> GetReviewSlotByIdAsync(Guid id);
        Task<List<ReviewSlotDto>> GetReviewSlotsByDate(DateOnly date);
        Task<ReviewSlotDto?> UpdateReviewSlotRoom(Guid id, string newRoom);
        Task<bool> DeleteReviewSlotAsync(Guid id);
    }
}
