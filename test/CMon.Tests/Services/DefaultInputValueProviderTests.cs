using System;
using System.IO;
using CMon.Services;
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
				.AddUserSecrets("CMon")
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

			var now = DateTime.UtcNow;

			// act
			var stats = provider.GetValues(0, now.AddHours(-2), now, 10);

			// assert
			Assert.NotNull(stats);
        }
    }
}
