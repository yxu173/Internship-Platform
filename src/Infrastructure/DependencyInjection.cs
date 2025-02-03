﻿using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Aggregates.Users;
using Domain.Repositories;
using Infrastructure.Authentication;
using Infrastructure.Authentication.Options;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Database")));

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<ITokenProvider, TokenProvider>();
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        //services.AddScoped<IEmailSender, EmailSender>();


        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


        var jwtSettings = new JwtSettings();
        configuration.Bind(nameof(JwtSettings), jwtSettings);
        var jwtSection = configuration.GetSection(nameof(JwtSettings));
        services.Configure<JwtSettings>(jwtSection);

        services.Configure<CookieAuthenticationOptions>(IdentityConstants.ExternalScheme, options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        });

        services.AddAuthentication(options =>
            {

             options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                // options.DefaultScheme = IdentityConstants.ApplicationScheme;
                // options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                // options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding
                        .UTF8.GetBytes(jwtSettings.SigningKey))
                };
            })
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                googleOptions.CallbackPath = "/signin-google";
                //googleOptions.SaveTokens = true;
            })
            .AddOAuth("GitHub", options =>
            {
                options.ClientId = configuration["Authentication:Github:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Github:ClientSecret"]!;
                options.CallbackPath = "/signin-github";
                options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                options.UserInformationEndpoint = "https://api.github.com/user";
                options.Scope.Add("user:email");
                options.CorrelationCookie.SameSite = SameSiteMode.None;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SaveTokens = true;
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        // Get user details from GitHub API
                        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                        response.EnsureSuccessStatusCode();

                        var userJson = await response.Content.ReadAsStringAsync();
                        var userData = JsonDocument.Parse(userJson).RootElement;

                        context.RunClaimActions(userData);
                        if (context.Identity?.FindFirst(ClaimTypes.Email) == null)
                        {
                            var emailsRequest =
                                new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                            emailsRequest.Headers.Authorization =
                                new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            emailsRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var emailsResponse =
                                await context.Backchannel.SendAsync(emailsRequest, context.HttpContext.RequestAborted);
                            emailsResponse.EnsureSuccessStatusCode();

                            var emailsJson = await emailsResponse.Content.ReadAsStringAsync();
                            var emailsData = JsonDocument.Parse(emailsJson).RootElement;

                            var primaryEmail = emailsData.EnumerateArray()
                                .FirstOrDefault(email => email.GetProperty("primary").GetBoolean())
                                .GetProperty("email").GetString();

                            if (primaryEmail != null)
                            {
                                context.Identity?.AddClaim(new Claim(ClaimTypes.Email, primaryEmail));
                            }
                        }
                    }
                };
            });
            services.AddAuthorization();

        return services;
    }
}