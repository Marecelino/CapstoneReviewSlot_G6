using Availability.Infrastructure.Persistence;
using Availability.Infrastructure.Repositories;
using Availability.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Availability.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:AvailabilityDb"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'AvailabilityDb' is not configured.");
            }

            services.AddDbContext<AvailabilityDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}