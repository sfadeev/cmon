using System.IO;
using CMon.Services;
using CMon.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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
		}

		[Fact]
		public void Test1()
		{
			// arrange
			var connectionString = new ConnectionStringOptions
			{
				DefaultConnection = Configuration.GetConnectionString("DefaultConnection")
			};
			var provider = new DefaultInputValueProvider(Options.Create(connectionString));

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
