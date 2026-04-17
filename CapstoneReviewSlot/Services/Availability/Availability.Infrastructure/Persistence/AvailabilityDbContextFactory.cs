using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Availability.Infrastructure.Persistence
{
    public class AvailabilityDbContextFactory
        : IDesignTimeDbContextFactory<AvailabilityDbContext>
    {
        public AvailabilityDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../Availability.Api"
            );

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AvailabilityDbContext>();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")
            );

            return new AvailabilityDbContext(optionsBuilder.Options);
        }
    }
}