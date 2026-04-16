using Identity.Domain.Interfaces;
using Identity.Infrastructure.Data;

namespace Identity.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IdentityDbContext _ctx;
    private UserRepository? _users;
    private LecturerRepository? _lecturers;

    public UnitOfWork(IdentityDbContext ctx) => _ctx = ctx;

    public IUserRepository Users => _users ??= new UserRepository(_ctx);
    public ILecturerRepository Lecturers => _lecturers ??= new LecturerRepository(_ctx);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _ctx.SaveChangesAsync(ct);
}
