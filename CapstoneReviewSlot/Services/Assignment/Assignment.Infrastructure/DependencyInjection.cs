using Assignment.Application.Services;
using Assignment.Domain.Interfaces.Repositories;
using Assignment.Domain.Interfaces.Services;
using Assignment.Infrastructure.Persistence;
using Assignment.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:AssignmentDb"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'AssignmentDb' is not configured.");
            }

            services.AddDbContext<AssignmentDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IReviewAssignmentRepository, ReviewAssignmentRepository>();
            services.AddScoped<IReviewAssignmentReviewerRepository, ReviewAssignmentReviewerRepository>();


            return services;
        }
    }

}
