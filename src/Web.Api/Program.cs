using Application;
using Domain.Aggregates.Users;
using Infrastructure;
using Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Web.Api;
using Web.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));


builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);


builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// for docker-compose
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     await dbContext.Database.MigrateAsync();

//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
//     string[] roles = new[] { "BASIC", "ADMIN" }; 

//     foreach (var role in roles)
//     {
//         if (!await roleManager.RoleExistsAsync(role))
//         {
//             var newRole = Role.Create(role);
//             await roleManager.CreateAsync(newRole);
//         }
//     }
// }

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


await app.RunAsync();
