using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Availability.Application.Interfaces;
using Availability.Application.Services;
using Availability.Application.Ultils;
using Availability.Infrastructure;
using Availability.Infrastructure.Common;
using Availability.Infrastructure.Interfaces;
using Availability.Infrastructure.Persistence;
using Availability.Infrastructure.Repositories;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Availability.Api.Architecture
{
    public static class IocContainer
    {
        public static IServiceCollection SetupIocContainer(this IServiceCollection services)
        {
            //Add Logger


            //Add Project Services
            services.SetupDbContext();
            services.SetupSwagger();

            //Add business services
            services.SetupBusinessServicesLayer();

            //Add HttpContextAccessor for role-based checks
            services.AddHttpContextAccessor();

            services.SetupJwt();
            // services.SetupGraphQl();
            return services;
        }

        public static IServiceCollection SetupBusinessServicesLayer(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICurrentTime, CurrentTime>();

            services.AddScoped<ILecturerAvailabilityRepository, LecturerAvailabilityRepository>();

            services.AddScoped<ILecturerAvailabilityService, LecturerAvailabilityService>();

            return services;
        }

        private static IServiceCollection SetupDbContext(this IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AvailabilityDbContext>(options =>
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly(typeof(AvailabilityDbContext).Assembly.FullName);
                    sql.CommandTimeout(300); // Cấu hình thời gian timeout truy vấn (tính bằng giây)
                    sql.EnableRetryOnFailure(
                        5,
                        TimeSpan.FromSeconds(10),
                        null
                    );
                })
            );

            return services;
        }

        public static IServiceCollection SetupSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ReviewCampaign API",
                    Version = "v1",
                    Description = "API for ReviewCampaign"
                });

                c.UseInlineDefinitionsForEnums();
                c.UseAllOfForInheritance();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Nhập token vào format: Bearer {your token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }

                    },
                    Array.Empty<string>()
                }
            });

                // Load XML comment
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


            return services;
        }

        private static IServiceCollection SetupJwt(this IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"] ??
                                                                            throw new InvalidOperationException())),
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                    x.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = ApiResult.Failure("401",
                                "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn.");
                            var json = JsonSerializer.Serialize(result);
                            return context.Response.WriteAsync(json);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = ApiResult.Failure("403",
                                "Bạn không có quyền truy cập vào tài nguyên này.");
                            var json = JsonSerializer.Serialize(result);
                            return context.Response.WriteAsync(json);
                        }
                    };
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Manager", policy =>
                    policy.RequireRole("Manager"));
                options.AddPolicy("Lecturer", policy =>
                    policy.RequireRole("Lecturer"));
                options.AddPolicy("Student", policy =>
                    policy.RequireRole("Student"));
            });

            return services;
        }
    }
}
