using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment.Domain.Dtos;
using Assignment.Domain.Entities;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Domain.Interfaces.Services;
using Assignment.Domain.Ultils;

namespace Assignment.Application.Services
{
    public class ReviewAssignmentReviewerService : IReviewAssignmentReviewerService
    {
        private readonly IReviewAssignmentReviewerRepository _repository;

        public ReviewAssignmentReviewerService(IReviewAssignmentReviewerRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ReviewAssignmentReviewerDto>> AddAsync(List<ReviewAssignmentReviewerDto> reviewers)
        {
            if (reviewers == null || reviewers.Count == 0)
            {
                throw ErrorHelper.BadRequest("Reviewer list is empty.");
            }

            var duplicatedLecturerInRequest = reviewers
                .GroupBy(x => new { x.LecturerId, x.ReviewAssignmentId })
                .Any(g => g.Count() > 1);

            if (duplicatedLecturerInRequest)
            {
                throw ErrorHelper.Conflict("Duplicate reviewer assignment in request.");
            }

            var duplicatedRoleInRequest = reviewers
                .GroupBy(x => new { x.ReviewAssignmentId, Role = NormalizeRole(x.Role) })
                .Any(g => g.Count() > 1);

            if (duplicatedRoleInRequest)
            {
                throw ErrorHelper.Conflict("Lecturer with same role can not be in the same assignment.");
            }

            var entities = new List<ReviewAssignmentReviewer>();

            foreach (var reviewer in reviewers)
            {
                var existed = await _repository.GetByReviewAssignmentIdAsync(reviewer.ReviewAssignmentId);

                if (existed.Any(x => x.LecturerId == reviewer.LecturerId))
                {
                    throw ErrorHelper.Conflict("A lecturer is already assigned to this review assignment.");
                }

                if (existed.Any(x => NormalizeRole(x.Role) == NormalizeRole(reviewer.Role)))
                {
                    throw ErrorHelper.Conflict("Lecturer with same role can not be in the same assignment.");
                }

                entities.Add(ToEntity(reviewer));
            }

            var created = await _repository.AddAsync(entities);
            return created.Select(ToDto).ToList();
        }

        public async Task<ReviewAssignmentReviewerDto> UpdateAsync(Guid id, ReviewAssignmentReviewerDto reviewer)
        {
            var current = await _repository.GetByIdAsync(id);
            if (current == null)
            {
                throw ErrorHelper.NotFound("Review assignment reviewer not found.");
            }

            var existed = await _repository.GetByReviewAssignmentIdAsync(reviewer.ReviewAssignmentId);

            if (existed.Any(x =>
                    x.Id != id &&
                    x.LecturerId == reviewer.LecturerId))
            {
                throw ErrorHelper.Conflict("A lecturer is already assigned to this review assignment.");
            }

            if (existed.Any(x =>
                    x.Id != id &&
                    NormalizeRole(x.Role) == NormalizeRole(reviewer.Role)))
            {
                throw ErrorHelper.Conflict("Lecturer with same role can not be in the same assignment.");
            }

            current.LecturerId = reviewer.LecturerId;
            current.ReviewAssignmentId = reviewer.ReviewAssignmentId;
            current.Role = reviewer.Role;

            var updated = await _repository.UpdateAsync(current);
            return ToDto(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var current = await _repository.GetByIdAsync(id);
            if (current == null)
            {
                throw ErrorHelper.NotFound("Review assignment reviewer not found.");
            }

            return await _repository.DeleteAsync(current);
        }

        public async Task<List<ReviewAssignmentReviewerDto>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(ToDto).ToList();
        }

        public async Task<ReviewAssignmentReviewerDto> GetByIdAsync(Guid id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null)
            {
                throw ErrorHelper.NotFound("Review assignment reviewer not found.");
            }

            return ToDto(data);
        }

        public async Task<List<ReviewAssignmentReviewerDto>> GetByReviewAssignmentIdAsync(Guid reviewAssignmentId)
        {
            var data = await _repository.GetByReviewAssignmentIdAsync(reviewAssignmentId);
            return data.Select(ToDto).ToList();
        }

        public async Task<List<ReviewAssignmentReviewerDto>> GetByLecturerIdAsync(Guid lecturerId)
        {
            var data = await _repository.GetByLecturerIdAsync(lecturerId);
            return data.Select(ToDto).ToList();
        }

        private static ReviewAssignmentReviewer ToEntity(ReviewAssignmentReviewerDto dto)
        {
            return new ReviewAssignmentReviewer
            {
                LecturerId = dto.LecturerId,
                ReviewAssignmentId = dto.ReviewAssignmentId,
                Role = dto.Role
            };
        }

        private static ReviewAssignmentReviewerDto ToDto(ReviewAssignmentReviewer entity)
        {
            return new ReviewAssignmentReviewerDto
            {
                LecturerId = entity.LecturerId,
                ReviewAssignmentId = entity.ReviewAssignmentId,
                Role = entity.Role
            };
        }

        private static string NormalizeRole(string role) => role.Trim().ToLowerInvariant();
    }
}
