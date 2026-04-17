using Assignment.Application.Services;
using Assignment.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IReviewAssignmentService, ReviewAssignmentService>();
            services.AddScoped<IReviewAssignmentReviewerService, ReviewAssignmentReviewerService>();
            return services;
        }
    }
}
