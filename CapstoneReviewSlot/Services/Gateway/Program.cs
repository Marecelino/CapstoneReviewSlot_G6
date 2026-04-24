var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseRouting();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger-identity/v1/swagger.json", "Identity API");
    c.SwaggerEndpoint("/swagger-session/v1/swagger.json", "Session API");
    c.SwaggerEndpoint("/swagger-availability/v1/swagger.json", "Availability API");
    c.SwaggerEndpoint("/swagger-registration/v1/swagger.json", "Registration API");
    c.SwaggerEndpoint("/swagger-assignment/v1/swagger.json", "Assignment API");
    c.SwaggerEndpoint("/swagger-report/v1/swagger.json", "Report API");
    c.RoutePrefix = "swagger"; // Hosts Swagger UI at /swagger
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
});

app.Run();
