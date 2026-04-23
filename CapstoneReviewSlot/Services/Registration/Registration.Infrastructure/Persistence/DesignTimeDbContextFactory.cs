using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Registration.Infrastructure.Persistence;

namespace Registration.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RegistrationDbContext>
{
    public RegistrationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RegistrationDbContext>();
        var connectionString = "Server=LAPTOP-MINHDA;Database=CapstoneReviewSlot_RegistrationDb;User Id=sa;Password=12345;Encrypt=True;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);
        return new RegistrationDbContext(optionsBuilder.Options);
    }
}
