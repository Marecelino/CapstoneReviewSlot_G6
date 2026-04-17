using Availability.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Infrastructure.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public ILecturerAvailabilityRepository LecturerAvailabilities { get; }

        Task<int> SaveChangesAsync();
        IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class;
        IQueryable<TResult> Select<T, TResult>(Expression<Func<T, TResult>> selector) where T : class;
    }
}
