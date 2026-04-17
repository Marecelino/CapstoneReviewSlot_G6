using Assignment.Domain.Dtos;
using Assignment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Domain.Interfaces.Services
{
    public interface IReviewAssignmentService
    {
        Task<ReviewAssignmentDto> AddAsync(ReviewAssignmentRequest assignment);
        Task<ReviewAssignmentDto> UpdateAsync(ReviewAssignmentRequest assignment);
        Task<bool> DeleteAsync(int id);
        Task<List<ReviewAssignmentDto>> GetAllAsync();
        Task<ReviewAssignmentDto> GetByIdAsync(Guid id);
        Task<List<ReviewAssignmentDto>> GetBySlotId(Guid slotId);
        Task<List<ReviewAssignmentDto>> GetByGroupId(Guid groupId);
    }
}
