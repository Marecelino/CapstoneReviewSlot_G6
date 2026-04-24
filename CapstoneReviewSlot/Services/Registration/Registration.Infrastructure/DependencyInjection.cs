using Microsoft.EntityFrameworkCore;
using Registration.Domain.Interfaces;
using Registration.Infrastructure.Persistence;
using Registration.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Registration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRegistrationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:RegistrationDb"]
            ?? throw new InvalidOperationException("Connection string 'RegistrationDb' is not configured.");

        services.AddDbContext<RegistrationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IStudentSlotPreferenceRepository, StudentSlotPreferenceRepository>();

        return services;
    }
}
