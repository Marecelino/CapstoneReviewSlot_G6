using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment.Domain.Entities;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Repositories
{
    public class ReviewAssignmentReviewerRepository : IReviewAssignmentReviewerRepository
    {
        private readonly AssignmentDbContext _context;

        public ReviewAssignmentReviewerRepository(AssignmentDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewAssignmentReviewer>> AddAsync(List<ReviewAssignmentReviewer> reviewers)
        {
            await _context.ReviewAssignmentReviewers.AddRangeAsync(reviewers);
            await SaveChangesAsync();
            return reviewers;
        }

        public async Task<ReviewAssignmentReviewer> UpdateAsync(ReviewAssignmentReviewer reviewer)
        {
            _context.ReviewAssignmentReviewers.Update(reviewer);
            await SaveChangesAsync();
            return reviewer;
        }

        public async Task<bool> DeleteAsync(ReviewAssignmentReviewer reviewer)
        {
            try
            {
                _context.ReviewAssignmentReviewers.Remove(reviewer);
                await SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ReviewAssignmentReviewer>> GetAllAsync()
        {
            return await _context.ReviewAssignmentReviewers
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<ReviewAssignmentReviewer?> GetByIdAsync(Guid id)
        {
            return await _context.ReviewAssignmentReviewers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<ReviewAssignmentReviewer>> GetByReviewAssignmentIdAsync(Guid reviewAssignmentId)
        {
            return await _context.ReviewAssignmentReviewers
                .Where(x => x.ReviewAssignmentId == reviewAssignmentId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<ReviewAssignmentReviewer>> GetByLecturerIdAsync(Guid lecturerId)
        {
            return await _context.ReviewAssignmentReviewers
                .Where(x => x.LecturerId == lecturerId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
