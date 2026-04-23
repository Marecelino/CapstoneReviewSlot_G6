using Identity.Application.Abstractions.Persistence;
using Identity.Application.Abstractions.Security;
using Identity.Application.Abstractions.Services;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence.Repositories;
using Identity.Infrastructure.Security;
using Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDb");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'IdentityDB' is not configured.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories
        services.AddScoped<IUserRepository,     UserRepository>();
        services.AddScoped<ILecturerRepository, LecturerRepository>();
        services.AddScoped<IPasswordResetOtpRepository, PasswordResetOtpRepository>();

        // Security
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IJwtTokenService,       JwtTokenService>();
        services.Configure<SmtpSettings>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}