using Availability.Domain.Dtos;
using Availability.Domain.Entities;
using Availability.Infrastructure.Interfaces;
using Availability.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Infrastructure.Repositories
{
    public class LecturerAvailabilityRepository : GenericRepository<LecturerAvailability>, ILecturerAvailabilityRepository
    {
        private readonly DbSet<LecturerAvailability> _dbSet;
        public LecturerAvailabilityRepository(
        AvailabilityDbContext dbContext,
        ICurrentTime timeService)
        : base(dbContext, timeService)
        {
            _dbSet = dbContext.LecturerAvailabilities;
        }

        public Task<List<LecturerAvailability>> GetByLecturerIdAsync(
            Guid id,
            Expression<Func<LecturerAvailability, bool>> predicate = null,
            params Expression<Func<LecturerAvailability, object>>[] includes)
        {
            IQueryable<LecturerAvailability> query = _dbSet
                .Where(x => !x.IsDeleted)
                .AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return query.Where(l => l.LecturerId == id).ToListAsync();
        }

        public Task<List<LecturerAvailability>> GetByReviewSlotIdAsync(
            Guid id,
            Expression<Func<LecturerAvailability, bool>> predicate = null,
            params Expression<Func<LecturerAvailability, object>>[] includes)
        {
            IQueryable<LecturerAvailability> query = _dbSet
                .Where(x => !x.IsDeleted)
                .AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            foreach (var include in includes)
                query = query.Include(include);

            return query.Where(l => l.ReviewSlotId == id).ToListAsync();
        }
    }
}
