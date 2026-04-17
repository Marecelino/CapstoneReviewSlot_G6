using Assignment.Domain.Entities;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Assignment.Infrastructure.Repositories
{
    internal class ReviewAssignmentRepository : IReviewAssignmentRepository
    {
        private readonly AssignmentDbContext _context;

        public ReviewAssignmentRepository(AssignmentDbContext context)
        {
            _context = context;
        }

        public async Task<ReviewAssignment> AddAsync(ReviewAssignment assignment)
        {
            await _context.ReviewAssignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<bool> DeleteAsync(ReviewAssignment assignment)
        {
            try
            {
                _context.Remove(assignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ReviewAssignment>> GetAllAsync(ReviewAssignment assignment)
        {
            return await _context.ReviewAssignments
                .OrderByDescending(r => r.ReviewOrder)
                .ToListAsync();
        }

        public async Task<List<ReviewAssignment>> GetByGroupId(Guid groupId)
        {
            return await _context.ReviewAssignments
                .OrderByDescending(r => r.ReviewOrder)
                .Where(r => r.CapstoneGroupId ==  groupId)
                .ToListAsync();
        }

        public async Task<ReviewAssignment> GetByIdAsync(Guid id)
        {
            return await _context.ReviewAssignments
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ReviewAssignment>> GetBySlotId(Guid slotId)
        {
            return await _context.ReviewAssignments
                .OrderByDescending(r => r.ReviewOrder)
                .Where(r => r.ReviewSlotId == slotId)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.ReviewAssignments
                .CountAsync();
        }

        public async Task<ReviewAssignment> UpdateAsync(ReviewAssignment assignment)
        {
            _context.ReviewAssignments.Update(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }
    }
}
