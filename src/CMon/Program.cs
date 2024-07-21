﻿using CMon.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.AspNetCore;
using Prometheus.Client.DependencyInjection;

namespace CMon
{
    public abstract class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddUserSecrets(typeof(Program).Assembly)
                .AddEnvironmentVariables();

            builder.Services
                .AddHttpClient()
                .AddMemoryCache()
                .Configure<AppOptions>(builder.Configuration.GetSection("Ccu"))
                .AddHostedService<PollingWorker>()
                .AddTransient<ICcuGateway, CcuGateway>()
                .AddTransient<IDevicePoller, DefaultDevicePoller>()
                .AddMetricFactory();
            
            var app = builder.Build();
            
            app.UsePrometheusServer();
        
            app.Run();
        }
    }
}