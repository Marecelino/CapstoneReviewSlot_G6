using Assignment.Domain.Dtos;
using Assignment.Domain.Entities;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Application.Services
{
    public class ReviewAssignmentService : IReviewAssignmentService
    {
        private readonly IReviewAssignmentRepository _repostory;
        private readonly IReviewAssignmentReviewerRepository _reviewerRepository;

        public ReviewAssignmentService(
            IReviewAssignmentRepository repostory,
            IReviewAssignmentReviewerRepository reviewerRepository)
        {
            _repostory = repostory;
            _reviewerRepository = reviewerRepository;
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

            return await GetToDtoAsync(created);
        }

        public async Task<bool> DeleteAsync(int id)
        {
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
            return await Task.WhenAll(assignments.Select(GetToDtoAsync)).ContinueWith(t => t.Result.ToList());
        }

        public async Task<List<ReviewAssignmentDto>> GetByGroupId(Guid groupId)
        {
            var assignments = await _repostory.GetByGroupId(groupId);
            return await Task.WhenAll(assignments.Select(GetToDtoAsync)).ContinueWith(t => t.Result.ToList());
        }

        public async Task<ReviewAssignmentDto> GetByIdAsync(Guid id)
        {
            var assignment = await _repostory.GetByIdAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException("Review assignment not found.");
            }

            return await GetToDtoAsync(assignment);
        }

        public async Task<List<ReviewAssignmentDto>> GetBySlotId(Guid slotId)
        {
            var assignments = await _repostory.GetBySlotId(slotId);
            return await Task.WhenAll(assignments.Select(GetToDtoAsync)).ContinueWith(t => t.Result.ToList());
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
            return await GetToDtoAsync(updated);
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

        private async Task<ReviewAssignmentDto> GetToDtoAsync(ReviewAssignment assignment)
        {
            var reviewers = await _reviewerRepository.GetByReviewAssignmentIdAsync(assignment.Id);

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
                UpdatedAtUtc = assignment.UpdatedAtUtc ?? assignment.CreatedAtUtc,
                Reviewers = reviewers.Select(x => new ReviewAssignmentReviewerDto
                {
                    LecturerId = x.LecturerId,
                    ReviewAssignmentId = x.ReviewAssignmentId,
                    Role = x.Role
                }).ToList()
            };
        }
    }
}
