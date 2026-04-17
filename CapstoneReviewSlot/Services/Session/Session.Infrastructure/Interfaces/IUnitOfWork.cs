using Session.Domain.Entities;
using System.Linq.Expressions;

namespace Session.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<ReviewCampaign> ReviewCampaigns { get; }
        public IGenericRepository<ReviewSlot> ReviewSlots { get; }

        Task<int> SaveChangesAsync();
        IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<TResult> Select<T, TResult>(Expression<Func<T, TResult>> selector) where T : class;
    }
}
