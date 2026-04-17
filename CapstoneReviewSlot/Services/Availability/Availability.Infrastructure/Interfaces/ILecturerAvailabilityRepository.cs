using Availability.Domain.Dtos;
using Availability.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Infrastructure.Interfaces
{
    public interface ILecturerAvailabilityRepository : IGenericRepository<LecturerAvailability>
    {
        Task<List<LecturerAvailability>> GetByLecturerIdAsync(Guid id, Expression<Func<LecturerAvailability, bool>> predicate = null,
        params Expression<Func<LecturerAvailability, object>>[] includes);
        Task<List<LecturerAvailability>> GetByReviewSlotIdAsync(Guid id, Expression<Func<LecturerAvailability, bool>> predicate = null,
        params Expression<Func<LecturerAvailability, object>>[] includes);
    }
}
