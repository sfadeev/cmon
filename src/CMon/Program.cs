using CMon.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus.Client.AspNetCore;
using Prometheus.Client.DependencyInjection;

namespace CMon
{
    internal abstract class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddUserSecrets(typeof(AppSettings).Assembly)
                .AddEnvironmentVariables();

            var services = builder.Services;

            services.AddHostedService<PollingWorker>();

            services.AddTransient<IDevicePoller, DefaultDevicePoller>();
            services.AddTransient<IDeviceRepository, DefaultDeviceRepository>();
            services.AddTransient<IInputValueProvider, DefaultInputValueProvider>();

            services.AddMetricFactory();
        
            // services.AddRazorPages();
            services.AddControllersWithViews();
            services.AddRouting(options => options.LowercaseUrls = true);

            var app = builder.Build();

            app.UseExceptionHandler("/Home/Error");

            // app.UseHsts();
            // app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            app.UsePrometheusServer();
        
            app.Run();
        }
    }
}