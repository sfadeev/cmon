using System.IO;
using CMon.Entities;
using CMon.Services;
using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CMon.Tests.Services
{
	public class DefaultInputValueProviderTests
	{
		public IConfigurationRoot Configuration { get; set; }

		public DefaultInputValueProviderTests()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddUserSecrets(Startup.UserSecretId)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			DataConnection.DefaultConfiguration = "Default";

			DataConnection.AddConfiguration(
				DataConnection.DefaultConfiguration,
				Configuration["Data:DefaultConnection:ConnectionString"],
				new PostgreSQLDataProvider("Default", PostgreSQLVersion.v93));
		}

		[Fact]
		public void Test1()
		{
			var connectionFactory = new DefaultDbConnectionFactory<DbContext, DbConnection>();
			var provider = new DefaultInputValueProvider(connectionFactory);

			// act
			var stats = provider.GetValues(new InputValueRequest
			{
				DeviceId = 0,
				BeginDate = "now-2y",
				EndDate = "now"
			});

			// assert
			Assert.NotNull(stats);
		}
	}
}
