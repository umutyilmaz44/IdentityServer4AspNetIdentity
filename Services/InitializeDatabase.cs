using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetShop.IdentityService.Data;
using NetShop.IdentityService.Models;
using NetShop.IdentityService.Settings;
using Serilog;

namespace NetShop.IdentityService.Services
{
    public static class InitializeDatabase
    {
        public static void MigrateDatabase(IApplicationBuilder app, IConfiguration configuration)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                InitIdentityConfig(serviceScope, configuration);
                InitIdentityUser(serviceScope, configuration);
            }
        }

        private static void InitIdentityConfig(IServiceScope serviceScope, IConfiguration configuration)
        {
            DbSettings dbSettings = configuration.GetSection(nameof(DbSettings)).Get<DbSettings>();
            if ((!string.IsNullOrEmpty(dbSettings.DatabaseType) && string.Equals(dbSettings.DatabaseType, "Postgresql", StringComparison.InvariantCultureIgnoreCase)) ||
                (!string.IsNullOrEmpty(dbSettings.DatabaseType) && string.Equals(dbSettings.DatabaseType, "Sqlite", StringComparison.InvariantCultureIgnoreCase)))
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.ApiResources)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var apiScope in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(apiScope.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        private static void InitIdentityUser(IServiceScope serviceScope, IConfiguration configuration)
        {
            var appDbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            appDbContext.Database.Migrate();

            var userMgr = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                                            new Claim(JwtClaimTypes.Role, "admin"),
                                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                                            new Claim(JwtClaimTypes.Role, "customer"),
                                            new Claim("location", "somewhere")
                                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");
            }
        }
    }
}