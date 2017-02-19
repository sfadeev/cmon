using AspNetCoreIdentity.Services;
using CMon.Services;
using CMon.Web.Entities;
using LinqToDB;
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
			DataConnection
				.AddConfiguration(
					"Default",
					// Configuration["Data:DefaultConnection:ConnectionString"],
					Configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>().DefaultConnection,
					new PostgreSQLDataProvider("Default", PostgreSQLVersion.v93));

			DataConnection.DefaultConfiguration = "Default";

			services.Configure<ConnectionStringOptions>(Configuration.GetSection("ConnectionStrings"));

			services.AddIdentity<DbUser, DbRole>(options =>
				{
					options.Cookies.ApplicationCookie.AuthenticationScheme = "ApplicationCookie";
					options.Cookies.ApplicationCookie.CookieName = "Interop";
					// options.Cookies.ApplicationCookie.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo("C:\\Github\\Identity\\artifacts"));
				})
				.AddUserStore<UserStore<DataContext, IdentityDbConnection, DbUser, DbRole, long, DbUserClaim, DbUserRole, DbUserLogin, DbUserToken, DbRoleClaim>>()
				.AddRoleStore<RoleStore<DataContext, IdentityDbConnection, DbRole, long, DbUserRole, DbRoleClaim>>()
				.AddLinqToDBConnectionFactory(new DefaultConnectionFactory<DataContext, IdentityDbConnection>(), typeof(long))
				.AddDefaultTokenProviders();

			// Add framework services.
			services.AddRouting(options => options.LowercaseUrls = true);
			services.AddMvc();

			// Add application services.
			services.AddTransient<IInputValueProvider, DefaultInputValueProvider>();

			services.AddTransient<IEmailSender, AuthMessageSender>();
			services.AddTransient<ISmsSender, AuthMessageSender>();
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

			// var connectionString = Configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>().DefaultConnection;
			/*var connectionString = new SqlConnectionStringBuilder(Configuration["Data:DefaultConnection:ConnectionString"])
			{
				InitialCatalog = "master"
			}.ConnectionString;*/

			// using (var db = new DataConnection(SqlServerTools.GetDataProvider(), connectionString))
			/*using (var db = new DataConnection(PostgreSQLTools.GetDataProvider(), connectionString))
			{
				try
				{
					var sql = "create database [" +
							  new SqlConnectionStringBuilder(Configuration["Data:DefaultConnection:ConnectionString"])
								  .InitialCatalog + "]";
					db.Execute(sql);
				}
				catch
				{
					//
				}

			}*/

			// Try to create tables
			/*using (var db = new IdentityDbConnection(connectionString))
			{
				TryCreateTable<ApplicationUser>(db);
				TryCreateTable<IdentityRole>(db);
				TryCreateTable<IdentityUserClaim<string>>(db);
				TryCreateTable<IdentityRoleClaim<string>>(db);
				TryCreateTable<IdentityUserLogin<string>>(db);
				TryCreateTable<IdentityUserRole<string>>(db);
				TryCreateTable<IdentityUserToken<string>>(db);
			}*/

			app.UseStaticFiles();

			// Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

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

		/*private void TryCreateTable<T>(IdentityDbConnection db)
			where T : class
		{
			try
			{
				db.CreateTable<T>();
			}
			catch
			{
				//
			}
		}*/
	}
}
