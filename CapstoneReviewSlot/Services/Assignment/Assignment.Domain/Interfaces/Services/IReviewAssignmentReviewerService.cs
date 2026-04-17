using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assignment.Domain.Dtos;

namespace Assignment.Domain.Interfaces.Services
{
    public interface IReviewAssignmentReviewerService
    {
        Task<List<ReviewAssignmentReviewerDto>> AddAsync(List<ReviewAssignmentReviewerDto> reviewers);
        Task<ReviewAssignmentReviewerDto> UpdateAsync(Guid id, ReviewAssignmentReviewerDto reviewer);
        Task<bool> DeleteAsync(Guid id);

        Task<List<ReviewAssignmentReviewerDto>> GetAllAsync();
        Task<ReviewAssignmentReviewerDto> GetByIdAsync(Guid id);
        Task<List<ReviewAssignmentReviewerDto>> GetByReviewAssignmentIdAsync(Guid reviewAssignmentId);
        Task<List<ReviewAssignmentReviewerDto>> GetByLecturerIdAsync(Guid lecturerId);
    }
}
