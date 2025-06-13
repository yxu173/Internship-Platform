using Application;
using Domain.Aggregates.Users;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Web.Api;
using Web.Api.Extensions;
using Web.Api.Hubs;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));


builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .AddSource("WebApi") // Matches the source name used in manual tracing if any
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: "web-api", serviceVersion: "1.0.0") // As defined in compose.yaml
            .AddTelemetrySdk())
        .AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true; // Enable recording of exceptions as trace events
        })
        .AddHttpClientInstrumentation() // Instrument outgoing HTTP requests
        .AddOtlpExporter(opts => // Export traces to OTLP endpoint
        {
            // Endpoint is typically configured via environment variable OTEL_EXPORTER_OTLP_ENDPOINT
            // but can be set here explicitly if needed.
            // opts.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint"));
        }))
    .WithMetrics(metricsProviderBuilder => metricsProviderBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: "web-api", serviceVersion: "1.0.0")
            .AddTelemetrySdk())
        .AddAspNetCoreInstrumentation() // Instrument ASP.NET Core for metrics
        .AddHttpClientInstrumentation() // Instrument outgoing HTTP requests for metrics
        .AddRuntimeInstrumentation()    // Collect runtime metrics (GC, JIT, etc.)
        .AddOtlpExporter(opts =>      // Export metrics to OTLP endpoint
        {
            // Endpoint is typically configured via environment variable OTEL_EXPORTER_OTLP_ENDPOINT
            // opts.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint"));
        }));

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json",
            "My API V1");
    });
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    string[] roles = new[] { "BASIC", "ADMIN" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var newRole = Role.Create(role);
            await roleManager.CreateAsync(newRole);
        }
    }
}

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseCors("Policy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/Notification");

await app.RunAsync();