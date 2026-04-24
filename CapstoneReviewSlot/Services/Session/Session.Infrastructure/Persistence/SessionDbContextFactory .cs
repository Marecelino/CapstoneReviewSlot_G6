using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Session.Infrastructure.Persistence
{
    public class SessionDbContextFactory
        : IDesignTimeDbContextFactory<SessionDbContext>
    {
        public SessionDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../Session.Api"
            );

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SessionDbContext>();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("SessionDb")
            );

            return new SessionDbContext(optionsBuilder.Options);
        }
    }
}
