using Assignment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Domain.Interfaces.Repositories
{
    public interface IReviewAssignmentRepository
    {
        Task<ReviewAssignment> AddAsync(ReviewAssignment assignment);
        Task<ReviewAssignment> UpdateAsync(ReviewAssignment assignment);
        Task<bool> DeleteAsync(ReviewAssignment assignment);
        Task<List<ReviewAssignment>> GetAllAsync(ReviewAssignment assignment);
        Task<ReviewAssignment> GetByIdAsync(Guid id);
        Task<List<ReviewAssignment>> GetBySlotId(Guid slotId);
        Task<List<ReviewAssignment>> GetByGroupId(Guid groupId);
        Task<int> GetCountAsync();
    }
}
