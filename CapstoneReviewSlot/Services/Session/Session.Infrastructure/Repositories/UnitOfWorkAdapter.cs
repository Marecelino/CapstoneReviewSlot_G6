using Session.Infrastructure.Interfaces;
using Session.Domain.Interfaces;
using Session.Infrastructure.Repositories;
using Session.Domain.Entities;
using Session.Infrastructure.Common;
using Session.Infrastructure.Persistence;
using System.Linq.Expressions;
using InfraUnitOfWork = Session.Infrastructure.Interfaces.IUnitOfWork;
using DomainUnitOfWork = Session.Domain.Interfaces.IUnitOfWork;

namespace Session.Infrastructure.Repositories;

/// <summary>
/// Bridges the old IUnitOfWork (from Session.Infrastructure.Interfaces) with the new
/// IUnitOfWork (from Session.Domain.Interfaces). ReviewCampaignService uses the old one;
/// new MediatR handlers use the new one. Both delegate to the same UnitOfWork.
/// </summary>
public class UnitOfWorkAdapter : DomainUnitOfWork, InfraUnitOfWork, IDisposable
{
    private readonly UnitOfWork _inner;
    private readonly SessionDbContext _db;
    private bool _disposed;

    public UnitOfWorkAdapter(UnitOfWork inner)
    {
        _inner = inner;
        _db = inner._dbContext;
    }

    // --- Session.Domain.Interfaces.IUnitOfWork ---
    IReviewCampaignRepository DomainUnitOfWork.Campaigns => _inner.Campaigns;
    IReviewSlotRepository DomainUnitOfWork.Slots => _inner.Slots;
    ICapstoneGroupRepository DomainUnitOfWork.CapstoneGroups => _inner.CapstoneGroups;
    ICapstoneGroupMemberRepository DomainUnitOfWork.CapstoneGroupMembers => _inner.CapstoneGroupMembers;
    Task<int> DomainUnitOfWork.SaveChangesAsync(CancellationToken ct) =>
        _inner.SaveChangesAsync(ct);

    // --- Session.Infrastructure.Interfaces.IUnitOfWork ---
    // Used by existing ReviewCampaignService via GenericRepository
    IGenericRepository<ReviewCampaign> InfraUnitOfWork.ReviewCampaigns =>
        new GenericRepository<ReviewCampaign>(_db, new CurrentTime());

    IGenericRepository<ReviewSlot> InfraUnitOfWork.ReviewSlots =>
        new GenericRepository<ReviewSlot>(_db, new CurrentTime());

    Task<int> InfraUnitOfWork.SaveChangesAsync() =>
        _inner.SaveChangesAsync();

    IQueryable<T> InfraUnitOfWork.Where<T>(Expression<Func<T, bool>> predicate) =>
        throw new NotSupportedException("Use specific repository methods instead.");

    IQueryable<TResult> InfraUnitOfWork.Select<T, TResult>(Expression<Func<T, TResult>> selector) =>
        throw new NotSupportedException("Use specific repository methods instead.");

    public void Dispose()
    {
        if (!_disposed)
        {
            _db.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
