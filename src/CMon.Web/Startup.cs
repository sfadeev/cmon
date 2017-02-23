using System.Globalization;
using AspNetCoreIdentity.Services;
using CMon.Extensions;
using CMon.Services;
using CMon.Web.Entities;
using CMon.Web.Services;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace CMon.Web
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			Log.Logger = LoggerBuilder.Build("cmon.web");

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

			// if (env.IsDevelopment())
			{
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

			// https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview
			// https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-management
			// https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers
			// services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("/tmp"));

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

			services.AddLocalization(options => options.ResourcesPath = "Resources");

			// Add framework services.
			services.AddRouting(options =>
			{
				options.AppendTrailingSlash = true;
				options.LowercaseUrls = true;
			});

			services.AddMvc()
				.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
				.AddDataAnnotationsLocalization();

			// Add application services.
			services.AddTransient<IDbConnectionFactory, DefaultDbConnectionFactory<IdentityDataContext>>();

			services.AddTransient<IXmlRepository, Linq2DbDataProtectionXmlRepository>();

			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();

			services.AddTransient<IInputValueProvider, DefaultInputValueProvider>();

			services.Configure<RequestLocalizationOptions>(options =>
			{
				options.DefaultRequestCulture = new RequestCulture("ru");
				options.SupportedCultures = options.SupportedUICultures = new[] { new CultureInfo("ru"), new CultureInfo("en") };
				options.RequestCultureProviders = new IRequestCultureProvider[]
				{
					new CookieRequestCultureProvider(), new AcceptLanguageHeaderRequestCultureProvider()
				};
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
			IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
		{
			loggerFactory
				// .AddConsole(Configuration.GetSection("Logging"))
				// .AddDebug()
				.AddSerilog();

			// Ensure any buffered events are sent at shutdown
			appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

			/*if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				// app.UseDatabaseErrorPage();
				// app.UseBrowserLink();
			}
			else*/
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseRequestLocalization(app.ApplicationServices
				.GetService<IOptions<RequestLocalizationOptions>>().Value);

			app.UseStaticFiles();

			app.UseIdentity();

			// Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
			app.UseGoogleAuthentication(new GoogleOptions
			{
				ClientId = Configuration["Authentication:Google:ClientId"],
				ClientSecret = Configuration["Authentication:Google:ClientSecret"]
				
			});

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
