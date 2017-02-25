using System.Globalization;
using CMon.Extensions;
using CMon.Services;
using CMon.Web.Entities;
using CMon.Web.Services;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Montr.Localization;
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
			services.Configure<GoogleOptions>(Configuration.GetSection("Authentication").GetSection(GoogleDefaults.AuthenticationScheme));
			services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailSender"));

			services.AddIdentity<DbUser, DbRole>(options =>
				{
					options.User.RequireUniqueEmail = true;
					options.SignIn.RequireConfirmedEmail = true;

					// options.Cookies.ApplicationCookie.AuthenticationScheme = "ApplicationCookie";
					// options.Cookies.ApplicationCookie.CookieName = "Interop";
					// options.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
					// options.Cookies.ApplicationCookie.AutomaticChallenge = true;
					// options.Cookies.ApplicationCookie.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo("C:\\Github\\Identity\\artifacts"));
				})
				.AddLinqToDBStores(new DefaultConnectionFactory<IdentityDataContext, IdentityDbConnection>(), typeof(long))
				.AddDefaultTokenProviders();

			services.AddLocalization(options => options.ResourcesPath = "Resources");

			// Add framework services.
			services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

			services.AddRouting(options =>
			{
				options.AppendTrailingSlash = true;
				options.LowercaseUrls = true;
			});

			services
				.AddMvc(options =>
				{
					options.Filters.Add(typeof(AutoValidateAntiforgeryTokenAuthorizationFilter));
					// options.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider());
				})
				.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
				.AddDataAnnotationsLocalization();

			// Add application services.
			services.AddTransient<IDbConnectionFactory, DefaultDbConnectionFactory<IdentityDataContext>>();

			services.AddTransient<IXmlRepository, Linq2DbDataProtectionXmlRepository>();

			services.AddTransient<IEmailSender, MailKitEmailSender>();
			services.AddTransient<ISmsSender, DefaultSmsSender>();

			services.AddTransient<IInputValueProvider, DefaultInputValueProvider>();

			services.AddSingleton<IValidationAttributeAdapterProvider, LocalizedValidationAttributeAdapterProvider>();

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

		public void Configure(IApplicationBuilder app,
			IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime,
			IAntiforgery antiforgery, IOptions<AntiforgeryOptions> antiforgeryOptions,
			IOptions<RequestLocalizationOptions> requestLocalizationOptions)
		{
			loggerFactory
				// .AddConsole(Configuration.GetSection("Logging"))
				// .AddDebug()
				.AddSerilog();

			// Ensure any buffered events are sent at shutdown
			appLifetime.ApplicationStopped.Register(() =>
			{
				Log.CloseAndFlush();
			});

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

			// app.UseValidateAntiForgeryToken();

			app.UseCors(builder => { });

			/*app.Use(next => context =>
				{
					if (
						string.Equals(context.Request.Path.Value, "/", StringComparison.OrdinalIgnoreCase) ||
						string.Equals(context.Request.Path.Value, "/index.html", StringComparison.OrdinalIgnoreCase))
					{
						// We can send the request token as a JavaScript-readable cookie, and Angular will use it by default.
						var tokens = antiforgery.GetAndStoreTokens(context);
						context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions() { HttpOnly = false });
					}

					return next(context);
				});*/

			app.UseRequestLocalization(requestLocalizationOptions.Value);

			app.UseStaticFiles();

			app.UseIdentity();

			app.UseGoogleAuthentication();
			app.UseCookieAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "page.index",
					template: "page/{id}",
					defaults: new { controller = "Page", action = "Index" });

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
