using DiplomServer.Application.Interfaces;
using DiplomServer.Application.Services;
using DiplomServer.Configuration;
using DiplomServer.Infrastructure.Auth;
using DiplomServer.Infrastructure.Data;
using DiplomServer.Infrastructure.Documents;
using DiplomServer.Infrastructure.Repositories;
using DiplomServer.Infrastructure.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace DiplomServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            return services;
        }

        public static IServiceCollection AddAppDb(this IServiceCollection services, IConfiguration configuration)
        {
            // == AppDbContext (Diplom) ==
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    )));

            // == ScheduleDbContext (schedule) ==
            var scheduleConnection = configuration.GetConnectionString("ScheduleConnection")
                ?? throw new InvalidOperationException("Connection string 'ScheduleConnection' not found.");

            services.AddDbContext<ScheduleDbContext>(options =>
                options.UseMySql(
                    scheduleConnection,
                    ServerVersion.AutoDetect(scheduleConnection),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    )));

            // == VrDbContext (vr) ==
            var vrConnection = configuration.GetConnectionString("VrConnection")
                ?? throw new InvalidOperationException("Connection string 'VrConnection' not found.");

            services.AddDbContext<VrDbContext>(options =>
                options.UseMySql(
                    vrConnection,
                    ServerVersion.AutoDetect(vrConnection),
                    mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    )));

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                            ?? throw new InvalidOperationException("Jwt configuration not found.");

            if (string.IsNullOrWhiteSpace(jwtOptions.Secret))
                throw new InvalidOperationException("Jwt:Secret is not configured.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            return services;
        }
        public static IServiceCollection AddAppSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var swaggerOptions = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>()
                                ?? new SwaggerOptions();

            services.Configure<SwaggerOptions>(configuration.GetSection(SwaggerOptions.SectionName));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(swaggerOptions.Version, new OpenApiInfo
                {
                    Title = swaggerOptions.Title,
                    Version = swaggerOptions.Version
                });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Вставь JWT токен в формате: Bearer {token}",

                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                jwtSecurityScheme,
                Array.Empty<string>()
            }
        });
            });

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRetakeDirectionService, RetakeDirectionService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IPdfService, PdfService>();

            services.AddScoped<JwtTokenGenerator>();
            services.AddScoped<PdfGenerator>();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            return services;
        }

        public static IServiceCollection AddAppRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IRetakeDirectionRepository, RetakeDirectionRepository>();
            services.AddScoped<ILookupRepository, LookupRepository>();

            return services;
        }
    }
}