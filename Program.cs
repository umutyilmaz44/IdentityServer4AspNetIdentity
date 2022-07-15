// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace NetShop.IdentityService
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(serilogConfiguration())
                .CreateLogger();

            try
            {
                string hostEnv = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")) ? "Development" : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                Log.Debug($"Environment['ASPNETCORE_ENVIRONMENT'] = {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
                Log.Debug($"IWebHostEnvironment['EnvironmentName'] = {hostEnv}");
                Log.Information("Starting host...");
                
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");

                throw ex;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) 
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration((hostContext, config) => {
                            var env = hostContext.HostingEnvironment;
                            config.SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                    .AddJsonFile($"appsettings.json", optional: false)
                                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                    .AddEnvironmentVariables();
                        })
                        .ConfigureWebHostDefaults(webBuilder => {
                            webBuilder                           
                                .ConfigureLogging(c => c.ClearProviders())
                                .UseStartup<Startup>();
                        })
                        .UseSerilog();
        }
        private static IConfiguration serilogConfiguration()
        {   
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            env = String.IsNullOrEmpty(env) ? "Development" : env;

            return new ConfigurationBuilder()
                        .SetBasePath(System.IO.Directory.GetCurrentDirectory()) 
                        .AddJsonFile($"appsettings.json", optional: false)
                        .AddJsonFile($"appsettings.{env}.json", optional: true)                      
                        .AddEnvironmentVariables()
                        .Build();
        }
    }
}