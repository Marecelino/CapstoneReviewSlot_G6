namespace Identity.Domain.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ILecturerRepository Lecturers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
