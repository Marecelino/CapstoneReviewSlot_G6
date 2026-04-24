using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Report.Application.Interfaces;
using Report.Application.Services;
using Report.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
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
});

var sessionBase = builder.Configuration["Services:Session"] ?? "http://session:80";
var assignmentBase = builder.Configuration["Services:Assignment"] ?? "http://assignment:80";
var identityBase = builder.Configuration["Services:Identity"] ?? "http://identity:80";

builder.Services.AddHttpClient<ISessionApiClient, SessionApiClient>(client =>
{
    client.BaseAddress = new Uri(sessionBase);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<IAssignmentApiClient, AssignmentApiClient>(client =>
{
    client.BaseAddress = new Uri(assignmentBase);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient<IIdentityApiClient, IdentityApiClient>(client =>
{
    client.BaseAddress = new Uri(identityBase);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddSingleton<IReportExporter, ReportExportService>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer not configured.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

