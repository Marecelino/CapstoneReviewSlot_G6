using Availability.Domain.Entities;
using Availability.Infrastructure.Interfaces;
using Availability.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AvailabilityDbContext _dbContext;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AvailabilityDbContext dbContext,
            ILecturerAvailabilityRepository lectureAvailabilities)
        {
            _dbContext = dbContext;
            LecturerAvailabilities = lectureAvailabilities;
        }

        public ILecturerAvailabilityRepository LecturerAvailabilities { get; }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        // Where
        public IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        // Select
        public IQueryable<TResult> Select<T, TResult>(Expression<Func<T, TResult>> selector) where T : class
        {
            return _dbContext.Set<T>().Select(selector);
        }

        // Transaction support
        public async Task BeginTransactionAsync()
        {
            if (_transaction != null) return;
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                if (_transaction != null)
                {
                    await _dbContext.SaveChangesAsync();
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackAsync()
        {
            try
            {
                if (_transaction != null)
                    await _transaction.RollbackAsync();
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
