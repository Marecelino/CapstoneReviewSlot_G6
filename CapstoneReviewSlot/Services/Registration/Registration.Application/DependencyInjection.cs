using Microsoft.Extensions.DependencyInjection;
using Registration.Application.Interfaces;
using Registration.Application.Services;

namespace Registration.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddRegistrationApplication(this IServiceCollection services)
    {
        services.AddScoped<IStudentSlotPreferenceService, StudentSlotPreferenceService>();
        return services;
    }
}
