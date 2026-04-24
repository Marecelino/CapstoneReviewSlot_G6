using Identity.Application;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Infrastructure;
using Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("Jwt:Key is not configured.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
                  ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.Migrate();

    // IMPORTANT: Use IPasswordHasherService (ASP.NET Identity PasswordHasher)
    // NOT IPasswordHasher (BCrypt) — AuthService.LoginAsync uses IPasswordHasherService,
    // so seeded passwords MUST use the same hasher to be verifiable.
    var passwordHasher = scope.ServiceProvider.GetRequiredService<Identity.Application.Abstractions.Security.IPasswordHasherService>();

    // --- Seed Admin account ---
    var existingAdmin = db.Users.FirstOrDefault(u => u.Email == "admin@admin.com");
    if (existingAdmin != null)
    {
        // Re-hash in case it was hashed with the wrong hasher (BCrypt vs ASP.NET Identity)
        existingAdmin.PasswordHash = passwordHasher.HashPassword(existingAdmin, "Admin@123");
        existingAdmin.Role = UserRole.Manager;
        db.SaveChanges();
    }
    else
    {
        var admin = User.Create("System Admin", "admin@admin.com", "", UserRole.Manager);
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");
        db.Users.Add(admin);
        db.SaveChanges();
    }

    // --- Seed Lecturer account ---
    var existingManager = db.Users.FirstOrDefault(u => u.Email == "manager@manager.com");
    if (existingManager != null)
    {
        existingManager.PasswordHash = passwordHasher.HashPassword(existingManager, "Manager@123");
        existingManager.Role = UserRole.Lecturer;
        db.SaveChanges();

        if (!db.Lecturers.Any(l => l.UserId == existingManager.Id))
        {
            var lecturer = Lecturer.Create(existingManager.Id, "MGR001", "Management");
            db.Lecturers.Add(lecturer);
            db.SaveChanges();
        }
    }
    else
    {
        var manager = User.Create("System Manager", "manager@manager.com", "", UserRole.Lecturer);
        manager.PasswordHash = passwordHasher.HashPassword(manager, "Manager@123");
        db.Users.Add(manager);
        db.SaveChanges();

        if (!db.Lecturers.Any(l => l.UserId == manager.Id))
        {
            var lecturer = Lecturer.Create(manager.Id, "MGR001", "Management");
            db.Lecturers.Add(lecturer);
            db.SaveChanges();
        }
    }
    // --- Seed List of Lecturers ---
    Identity.Api.Extensions.DataSeeder.SeedLecturers(db, passwordHasher);
}

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();