
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetShop.IdentityService.Data;
using NetShop.IdentityService.Models;
using NetShop.IdentityService.Settings;
using Newtonsoft.Json;
using Serilog;

namespace NetShop.IdentityService.Services
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            DbSettings dbSettings = configuration.GetSection(nameof(DbSettings)).Get<DbSettings>();

            Log.Debug($"{nameof(DbSettings)}: {Environment.NewLine}{JsonConvert.SerializeObject(dbSettings, Formatting.Indented)}");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            if (!string.IsNullOrEmpty(dbSettings.DatabaseType) && string.Equals(dbSettings.DatabaseType, "Postgresql", StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(dbSettings.PostgresqlSettings.ConnectionString));
            }
            else if (!string.IsNullOrEmpty(dbSettings.DatabaseType) && string.Equals(dbSettings.DatabaseType, "Sqlite", StringComparison.InvariantCultureIgnoreCase))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(dbSettings.SqliteSettings.ConnectionString));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName: dbSettings.PostgresqlSettings.Database)
                    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    // Logging sql 
                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();
                });
            }
        }

        public static void AddIdentityServerRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            DbSettings dbSettings = configuration.GetSection(nameof(DbSettings)).Get<DbSettings>();

            var builder = services.AddIdentityServer(options =>
                           {
                               options.Events.RaiseErrorEvents = true;
                               options.Events.RaiseInformationEvents = true;
                               options.Events.RaiseFailureEvents = true;
                               options.Events.RaiseSuccessEvents = true;

                               // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                               options.EmitStaticAudienceClaim = true;
                           })
                           .AddAspNetIdentity<ApplicationUser>()
                           .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>();

            if (!string.IsNullOrEmpty(dbSettings.DatabaseType) && string.Equals(dbSettings.DatabaseType, "Postgresql", StringComparison.InvariantCultureIgnoreCase))
            {
                var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

                // For Postgresql Usage
                builder.AddConfigurationStore(options => {
                            options.ConfigureDbContext = cdbc => {
                                cdbc.UseNpgsql(dbSettings.PostgresqlSettings.ConnectionString, 
                                    sqlOptions => sqlOptions.MigrationsAssembly(migrationAssembly));
                            };
                        })
                        .AddOperationalStore(options => {
                            options.ConfigureDbContext = cdbc => {
                                cdbc.UseNpgsql(dbSettings.PostgresqlSettings.ConnectionString, 
                                    sqlOptions => sqlOptions.MigrationsAssembly(migrationAssembly));
                            };
                        });
            }
            else if (!string.IsNullOrEmpty(dbSettings.DatabaseType) && string.Equals(dbSettings.DatabaseType, "Sqlite", StringComparison.InvariantCultureIgnoreCase))
            {
                var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                // For Sqlite Usage
                builder.AddConfigurationStore(options => {
                            options.ConfigureDbContext = cdbc => {
                                cdbc.UseSqlite(dbSettings.SqliteSettings.ConnectionString, 
                                    sqlOptions => sqlOptions.MigrationsAssembly(migrationAssembly));
                            };
                        })
                        .AddOperationalStore(options => {
                            options.ConfigureDbContext = cdbc => {
                                cdbc.UseSqlite(dbSettings.SqliteSettings.ConnectionString, 
                                    sqlOptions => sqlOptions.MigrationsAssembly(migrationAssembly));
                            };
                        });
            }
            else
            {
                 // For InMemory Usage
                builder.AddInMemoryIdentityResources(Config.IdentityResources)
                        .AddInMemoryApiResources(Config.ApiResources)
                        .AddInMemoryApiScopes(Config.ApiScopes)
                        .AddInMemoryClients(Config.Clients);
            }
            
            string hostEnv = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) ? "Development" : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (hostEnv == "Development")
            {
                // not recommended for production - you need to store your key material somewhere secure
                builder.AddDeveloperSigningCredential();
            }
        }
    }
}