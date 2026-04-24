using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Session.Application.Interfaces;
using Session.Application.Services;
using Session.Application.Ultils;
using Session.Domain.Interfaces;
using Session.Infrastructure;
using Session.Infrastructure.Common;
using Session.Infrastructure.Interfaces;
using Session.Infrastructure.Persistence;
using Session.Infrastructure.Repositories;
using IdentityDbContext = Identity.Infrastructure.Persistence.IdentityDbContext;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DomainUnitOfWork = Session.Domain.Interfaces.IUnitOfWork;
using RepositoryUnitOfWork = Session.Infrastructure.Repositories.UnitOfWork;
using InfraUnitOfWork = Session.Infrastructure.Interfaces.IUnitOfWork;
using UnitOfWorkAdapter = Session.Infrastructure.Repositories.UnitOfWorkAdapter;
namespace Session.Api.Architecture
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
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // For MediatR handlers using Session.Domain.Interfaces.IUnitOfWork
            services.AddScoped<DomainUnitOfWork, RepositoryUnitOfWork>();

            // Bridge old and new IUnitOfWork — ReviewCampaignService uses Infra IUnitOfWork
            services.AddScoped<InfraUnitOfWork>(sp =>
            {
                var inner = (RepositoryUnitOfWork)sp.GetRequiredService<DomainUnitOfWork>();
                return new UnitOfWorkAdapter(inner);
            });

            services.AddScoped<ICurrentTime, CurrentTime>();

            services.AddScoped<IReviewCampaignService, ReviewCampaignService>();
            services.AddScoped<IReviewSlotService, ReviewSlotService>();

            // CapstoneGroup services
            services.AddScoped<ICapstoneGroupService, CapstoneGroupService>();
            services.AddScoped<IExcelImportService, ExcelImportService>();
            services.AddScoped<ILecturerNameMapper, LecturerNameMapper>();

            return services;
        }

        private static IServiceCollection SetupDbContext(this IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("SessionDb");

            services.AddDbContext<SessionDbContext>(options =>
                options.UseSqlServer(connectionString, sql =>
                {
                    sql.MigrationsAssembly(typeof(SessionDbContext).Assembly.FullName);
                    sql.CommandTimeout(300);
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                })
            );

            // Share IdentityDbContext for lecturer name resolution
            // This must point to the Identity database where Lecturer/User tables exist
            var identityConnectionString = configuration.GetConnectionString("IdentityDb");
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(identityConnectionString, sql =>
                {
                    sql.CommandTimeout(60);
                    sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                }));

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
                        NameClaimType = ClaimTypes.NameIdentifier,
                        RoleClaimType = ClaimTypes.Role
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
