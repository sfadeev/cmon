using System;
using CMon.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Prometheus.Client.AspNetCore;
using Prometheus.Client.DependencyInjection;
using Serilog;
using Serilog.Extensions.Logging;

namespace CMon
{
    public abstract class Program
    {
        public const string ConfigFilePath = "./data/settings.json";
        
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            var logger = new SerilogLoggerFactory(Log.Logger).CreateLogger<Program>();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                
                builder.Configuration.AddJsonFile(ConfigFilePath, true, true);

                builder.Configuration
                    .AddUserSecrets(typeof(Program).Assembly)
                    .AddEnvironmentVariables();
                
                builder.Services
                    .AddSerilog((services, lc) => lc
                        .ReadFrom.Configuration(builder.Configuration)
                        .ReadFrom.Services(services));

                builder.Services
                    .AddHttpClient()
                    .AddMemoryCache()
                    .Configure<AppOptions>(builder.Configuration.GetSection("Ccu"))
                    .AddHostedService<PollingWorker>()
                    .AddTransient<ICcuGateway, CcuGateway>()
                    .AddTransient<IDevicePoller, DefaultDevicePoller>()
                    .AddMetricFactory();

                var app = builder.Build();
                
                app.UseSerilogRequestLogging();

                app.UsePrometheusServer();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}