using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment.Domain.Entities;

namespace Assignment.Domain.Interfaces.Repositories
{
    public interface IReviewAssignmentReviewerRepository
    {
        Task<List<ReviewAssignmentReviewer>> AddAsync(List<ReviewAssignmentReviewer> reviewers);
        Task<ReviewAssignmentReviewer> UpdateAsync(ReviewAssignmentReviewer reviewer);
        Task<bool> DeleteAsync(ReviewAssignmentReviewer reviewer);

        Task<List<ReviewAssignmentReviewer>> GetAllAsync();
        Task<ReviewAssignmentReviewer?> GetByIdAsync(Guid id);

        Task<List<ReviewAssignmentReviewer>> GetByReviewAssignmentIdAsync(Guid reviewAssignmentId);
        Task<List<ReviewAssignmentReviewer>> GetByLecturerIdAsync(Guid lecturerId);

        Task<int> SaveChangesAsync();
    }
}
