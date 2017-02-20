using AspNetCoreIdentity.Services;
using CMon.Services;
using CMon.Web.Entities;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMon.Web
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			// if (env.IsDevelopment())
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets(UserSecret.Id);
			}

			builder.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Set connection configuration
			var connectionStringsSection = Configuration.GetSection("ConnectionStrings");

			DataConnection
				.AddConfiguration(
					"Default",
					// Configuration["Data:DefaultConnection:ConnectionString"],
					connectionStringsSection.Get<ConnectionStringOptions>().DefaultConnection,
					new PostgreSQLDataProvider("Default", PostgreSQLVersion.v93));

			DataConnection.DefaultConfiguration = "Default";

			services.Configure<ConnectionStringOptions>(connectionStringsSection);

			services.AddIdentity<DbUser, DbRole>(/*options =>
				{
					options.Cookies.ApplicationCookie.AuthenticationScheme = "ApplicationCookie";
					options.Cookies.ApplicationCookie.CookieName = "Interop";
					options.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
					options.Cookies.ApplicationCookie.AutomaticChallenge = true;
					options.Cookies.ApplicationCookie.DataProtectionProvider =
						DataProtectionProvider.Create(new DirectoryInfo("C:\\Github\\Identity\\artifacts"));
				}*/)
				.AddLinqToDBStores(new DefaultConnectionFactory<IdentityDataContext, IdentityDbConnection>(), typeof(long))
				.AddDefaultTokenProviders();

			// Add framework services.
			services.AddRouting(options =>
			{
				options.AppendTrailingSlash = true;
				options.LowercaseUrls = true;
			});

			services.AddMvc();

			// Add application services.

			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();

			services.AddTransient<IInputValueProvider, DefaultInputValueProvider>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				// app.UseDatabaseErrorPage();
				// app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseIdentity();

			// Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

			app.UseCookieAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "device.index",
					template: "d/{deviceId:long}",
					defaults: new { controller = "Device", action = "Index" });

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
