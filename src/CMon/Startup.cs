using System;
using System.Globalization;
using System.Reflection;
using CMon.Entities;
using CMon.Extensions;
using CMon.Hubs;
using CMon.Services;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using LinqToDB.Identity;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Montr.Localization;
using Newtonsoft.Json;
using Serilog;

namespace CMon
{
	public class Startup
	{
		public const string UserSecretId = "cmon";

		public Startup(IHostingEnvironment env)
		{
			Log.Logger = LoggerBuilder.Build(env, "cmon");

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

			// if (env.IsDevelopment())
			{
				builder.AddUserSecrets(UserSecretId);
			}

			builder.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var connectionString = Configuration["Data:DefaultConnection:ConnectionString"];

			DataConnection.DefaultConfiguration = "Default";

			DataConnection.AddConfiguration(
				DataConnection.DefaultConfiguration, connectionString, new PostgreSQLDataProvider(PostgreSQLVersion.v93));

			// configure options
			services.Configure<GoogleOptions>(Configuration.GetSection("Authentication").GetSection(GoogleDefaults.AuthenticationScheme));

			services.Configure<DeviceOptions>(Configuration.GetSection("DeviceOptions"));
			services.Configure<EmailSenderOptions>(Configuration.GetSection("EmailSender"));

			services.Configure<RequestLocalizationOptions>(options =>
			{
				options.DefaultRequestCulture = new RequestCulture("ru");
				options.SupportedCultures = options.SupportedUICultures = new[] { new CultureInfo("ru"), new CultureInfo("en") };
				options.RequestCultureProviders = new IRequestCultureProvider[]
				{
					new CookieRequestCultureProvider(), new AcceptLanguageHeaderRequestCultureProvider()
				};
			});

			// Add framework services.
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
				.AddLinqToDBStores(new DefaultConnectionFactory<DbContext, DbConnection>(), typeof(long))
				.AddDefaultTokenProviders();
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(o => o.LoginPath = new PathString("/login"))
				.AddGoogle(o =>
				{
					o.ClientId = Configuration["Authentication:Google:ClientId"];
					o.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
				})
				/*.AddFacebook(o =>
				{
					o.AppId = Configuration["facebook:appid"];
					o.AppSecret = Configuration["facebook:appsecret"];
				})*/;

			services.AddLocalization(options => options.ResourcesPath = "Resources");

			services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

			services.AddRouting(options =>
			{
				options.AppendTrailingSlash = true;
				options.LowercaseUrls = true;
			});

			services.AddMvc(options =>
				{
					options.Filters.Add(typeof(AutoValidateAntiforgeryTokenAuthorizationFilter));
					// options.ModelMetadataDetailsProviders.Add(new CustomMetadataProvider());
				})
				.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
				.AddDataAnnotationsLocalization();

			services.AddSignalR();

			services.AddSingleton(typeof(DefaultHubLifetimeManager<>), typeof(DefaultHubLifetimeManager<>));
			services.AddSingleton(typeof(HubLifetimeManager<>), typeof(DefaultPresenceHublifetimeManager<>));
			services.AddSingleton(typeof(IUserTracker<>), typeof(InMemoryUserTracker<>));

			//services.AddSingleton(typeof(RedisHubLifetimeManager<>), typeof(RedisHubLifetimeManager<>));
			//services.AddSingleton(typeof(HubLifetimeManager<>), typeof(RedisPresenceHublifetimeManager<>));
			//services.AddSingleton(typeof(IUserTracker<>), typeof(RedisUserTracker<>));

			// Hangfire
			services.AddHangfire(configuration =>
			{
				configuration.UsePostgreSqlStorage(connectionString,
					new PostgreSqlStorageOptions
					{
						PrepareSchemaIfNecessary = false
					});
			});
			JobHelper.SetSerializerSettings(
				new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
			services.AddSingleton<IHttpClientFactory, DefaultHttpClientFactory>();

			// Infrastructure services
			services.AddSingleton<IBackgroundJob, HangfireBackgroundJob>();
			services.AddSingleton<IValidationAttributeAdapterProvider, LocalizedValidationAttributeAdapterProvider>();

			services.AddTransient<IDbConnectionFactory, DefaultDbConnectionFactory<DbContext, DbConnection>>();
			services.AddTransient<IXmlRepository, Linq2DbDataProtectionXmlRepository>();
			services.AddTransient<IEmailSender, MailKitEmailSender>();
			services.AddTransient<ISmsSender, DefaultSmsSender>();

			// todo: register by device type
			services.AddSingleton<IDeviceManagerFactory, DefaultDeviceManagerFactory>();
			services.AddTransient<CcuDeviceManager, CcuDeviceManager>();
			
			// Application services
			// services.AddSingleton<IStartable, DevicePollingStarter>();
			services.AddSingleton<IIdentityProvider, ClaimsIdentityProvider>();

            services.AddSingleton<Sha1Hasher, Sha1Hasher>();
            services.AddSingleton<ICcuGateway, CcuGateway>();
            services.AddTransient<IDeviceRepository, DefaultDeviceRepository>();

			services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
		}

		public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger, IApplicationLifetime appLifetime,
			IAntiforgery antiforgery, IOptions<AntiforgeryOptions> antiforgeryOptions, IOptions<RequestLocalizationOptions> requestLocalizationOptions)
		{
			loggerFactory
				// .AddConsole(Configuration.GetSection("Logging"))
				// .AddDebug()
				.AddSerilog();

			appLifetime.ApplicationStarted.Register(() =>
			{
			    try
			    {
                    logger.LogDebug("ApplicationStarted...");

                    foreach (var startable in app.ApplicationServices.GetServices<IStartable>())
                    {
                        logger.LogDebug("Starting service {0}", startable);

                        startable.Start();
                    }
                }
			    catch (Exception ex)
			    {
                    logger.LogError(0, ex, "Error occured in ApplicationStarted.");

                    throw;
			    }
			});

		    appLifetime.ApplicationStopping.Register(() =>
		    {
		        try
		        {
		            logger.LogDebug("ApplicationStopping...");

		            foreach (var startable in app.ApplicationServices.GetServices<IStartable>())
		            {
		                logger.LogDebug("Stopping service {0}", startable);

		                startable.Stop();
		            }
		        }
		        catch (Exception ex)
		        {
		            logger.LogError(0, ex, "Error occured in ApplicationStopping.");

		            throw;
		        }
		    });

		    appLifetime.ApplicationStopped.Register(() =>
		    {
		        try
		        {
		            logger.LogDebug("ApplicationStopped...");

		            Log.CloseAndFlush();
		        }
		        catch (Exception ex)
		        {
		            logger.LogError(0, ex, "Error occured in ApplicationStopped.");

		            throw;
		        }
		    });

			/*if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				// app.UseDatabaseErrorPage();
				// app.UseBrowserLink();
			}
			else*/
			{
				app.UseExceptionHandler("/home/error");
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

			app.UseAuthentication();

			// app.UseGoogleAuthentication();
			// app.UseCookieAuthentication();

			app.UseSignalR(routes =>
			{
				routes.MapHub<Chat>("chat");
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "page.index",
					template: "page/{id}",
					defaults: new { controller = "Page", action = "Index" });

				/*routes.MapRoute(
					name: "device.index",
					template: "d/{deviceId:long}",
					defaults: new { controller = "Device", action = "Index" });*/

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseHangfireServer(new BackgroundJobServerOptions
			{
				WorkerCount = 5
			});

			app.UseHangfireDashboard(options: new DashboardOptions
			{
				Authorization = new IDashboardAuthorizationFilter[]
				{
					new HangfireDashboardAuthorizationFilter()
				}
			});
		}

		public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
		{
			public bool Authorize(DashboardContext context)
			{
				var user = context.GetHttpContext().User;

				return user.Identity.IsAuthenticated && user.Identity.Name == "fadeev@gmail.com";
			}
		}
	}
}
