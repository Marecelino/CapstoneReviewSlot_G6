using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Session.Domain.Interfaces;
using Session.Infrastructure.Persistence;
using Session.Infrastructure.Repositories;

namespace Session.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SessionDb");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'SessionDb' is not configured.");
            }

            services.AddDbContext<SessionDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, Repositories.UnitOfWork>();

            return services;
        }
    }
}
