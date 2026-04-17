using Assignment.Domain.Dtos;
using Assignment.Domain.Entities;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Application.Services
{
    public class ReviewAssignmentService : IReviewAssignmentService
    {
        private readonly IReviewAssignmentRepository _repostory;

        public ReviewAssignmentService(IReviewAssignmentRepository repostory)
        {
            _repostory = repostory;
        }

        public async Task<ReviewAssignmentDto> AddAsync(ReviewAssignmentRequest assignment)
        {
            var existedInSlot = await _repostory.GetBySlotId(assignment.ReviewSlotId);
            if (existedInSlot.Any(x => x.CapstoneGroupId == assignment.CapstoneGroupId))
            {
                throw new InvalidOperationException("This group is already assigned in the selected review slot.");
            }

            var entity = await GetToEntityAsync(assignment);
            var created = await _repostory.AddAsync(entity);

            return GetToDto(created);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Current service contract uses int, while entity key is Guid.
            // Use ReviewOrder as the deletion key to satisfy existing interface.
            var all = await _repostory.GetAllAsync(new ReviewAssignment());
            var assignment = all.FirstOrDefault(x => x.ReviewOrder == id);

            if (assignment == null)
            {
                return false;
            }

            return await _repostory.DeleteAsync(assignment);
        }

        public async Task<List<ReviewAssignmentDto>> GetAllAsync()
        {
            var assignments = await _repostory.GetAllAsync(new ReviewAssignment());
            return assignments.Select(GetToDto).ToList();
        }

        public async Task<List<ReviewAssignmentDto>> GetByGroupId(Guid groupId)
        {
            var assignments = await _repostory.GetByGroupId(groupId);
            return assignments.Select(GetToDto).ToList();
        }

        public async Task<ReviewAssignmentDto> GetByIdAsync(Guid id)
        {
            var assignment = await _repostory.GetByIdAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException("Review assignment not found.");
            }

            return GetToDto(assignment);
        }

        public async Task<List<ReviewAssignmentDto>> GetBySlotId(Guid slotId)
        {
            var assignments = await _repostory.GetBySlotId(slotId);
            return assignments.Select(GetToDto).ToList();
        }

        public async Task<ReviewAssignmentDto> UpdateAsync(ReviewAssignmentRequest assignment)
        {
            var sameGroupAssignments = await _repostory.GetByGroupId(assignment.CapstoneGroupId);
            var current = sameGroupAssignments
                .FirstOrDefault(x => x.ReviewSlotId == assignment.ReviewSlotId);

            if (current == null)
            {
                throw new KeyNotFoundException("Review assignment not found for the provided group and slot.");
            }

            current.AssignedBy = assignment.AssignedBy;
            current.AssignedAt = DateTime.UtcNow;

            var updated = await _repostory.UpdateAsync(current);
            return GetToDto(updated);
        }

        private async Task<ReviewAssignment> GetToEntityAsync(ReviewAssignmentRequest assignment)
        {
            var inSameSlot = await _repostory.GetBySlotId(assignment.ReviewSlotId);
            var nextOrder = inSameSlot.Count == 0 ? 1 : inSameSlot.Max(x => x.ReviewOrder) + 1;

            return new ReviewAssignment
            {
                CapstoneGroupId = assignment.CapstoneGroupId,
                ReviewSlotId = assignment.ReviewSlotId,
                AssignedBy = assignment.AssignedBy,
                AssignedAt = DateTime.UtcNow,
                Status = "Assigned",
                ReviewOrder = nextOrder
            };
        }

        private static ReviewAssignmentDto GetToDto(ReviewAssignment assignment)
        {
            return new ReviewAssignmentDto
            {
                Id = assignment.Id,
                CapstoneGroupId = assignment.CapstoneGroupId,
                ReviewSlotId = assignment.ReviewSlotId,
                Status = assignment.Status,
                ReviewOrder = assignment.ReviewOrder,
                AssignedBy = assignment.AssignedBy,
                AssignedAt = assignment.AssignedAt,
                CreatedAtUtc = assignment.CreatedAtUtc,
                UpdatedAtUtc = assignment.UpdatedAtUtc ?? assignment.CreatedAtUtc
            };
        }
    }
}
