using Entities;
using Microsoft.EntityFrameworkCore;
using Session.Infrastructure.Interfaces;
using Session.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Session.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
     where TEntity : BaseEntity
    {
        private readonly SessionDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ICurrentTime _timeService;

        public GenericRepository(SessionDbContext dbContext, ICurrentTime timeService)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
            _timeService = timeService;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {

            // Chuyển tất cả các trường DateTime thành UTC
            entity.CreatedAtUtc = _timeService.GetCurrentTime().ToUniversalTime();
            entity.UpdatedAtUtc = _timeService.GetCurrentTime().ToUniversalTime();


            var result = await _dbSet.AddAsync(entity);
            return result.Entity;
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedAtUtc = _timeService.GetCurrentTime().ToUniversalTime();
                entity.UpdatedAtUtc = _timeService.GetCurrentTime().ToUniversalTime(); // Nếu có trường UpdatedAt
            }

            await _dbSet.AddRangeAsync(entities);
        }


        public Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            if (predicate != null) query = query.Where(predicate);
            foreach (var include in includes) query = query.Include(include);

            return query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (var include in includes) query = query.Include(include);
            var result = await query.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<bool> SoftRemove(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedAtUtc = _timeService.GetCurrentTime().ToUniversalTime();

            _dbSet.Update(entity);
            return true;
        }

        public async Task<bool> HardRemoveAsyn(TEntity entitiy)
        {
            _dbSet.RemoveRange(entitiy);
            return true;
        }


        public async Task<bool> SoftRemoveRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.UpdatedAtUtc = _timeService.GetCurrentTime();
            }

            _dbSet.UpdateRange(entities);
            //  await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SoftRemoveRangeById(List<Guid> entitiesId) // update hàng loạt cùng 1 trường thì làm y chang
        {
            var entities = await _dbSet.Where(e => entitiesId.Contains(e.Id)).ToListAsync();

            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.UpdatedAtUtc = _timeService.GetCurrentTime();
            }

            _dbContext.UpdateRange(entities);
            return true;
        }

        public async Task<bool> Update(TEntity entity)
        {
            entity.UpdatedAtUtc = _timeService.GetCurrentTime();
            _dbSet.Update(entity);
            //   await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRange(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdatedAtUtc = _timeService.GetCurrentTime();
            }

            _dbSet.UpdateRange(entities);
            //  await _dbContext.SaveChangesAsync();
            return true;
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _dbSet;
        }

        public async Task<TEntity?> FirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;

            // Include các navigation properties
            foreach (var include in includes) query = query.Include(include);

            // Áp dụng predicate (nếu có)
            if (predicate != null) query = query.Where(predicate);

            // Lấy bản ghi đầu tiên
            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> HardRemove(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var entities = await _dbSet.Where(predicate).ToListAsync();
                if (entities.Any())
                {
                    _dbSet.RemoveRange(entities);
                    return true;
                }

                return false; // Không có gì để xóa
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while performing hard remove: {ex.Message}");
            }
        }

        public async Task<bool> HardRemoveRange(List<TEntity> entities)
        {
            try
            {
                if (entities.Any())
                {
                    _dbSet.RemoveRange(entities);
                    return true;
                }

                return false; // Không có gì để xóa
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while performing hard remove range: {ex.Message}");
            }
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return _dbSet.CountAsync(predicate);
            }
            catch (Exception e)
            {
                throw new Exception($"Error while performing: {e.Message}");
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
