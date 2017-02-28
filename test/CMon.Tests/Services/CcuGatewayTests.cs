using System.IO;
using CMon.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CMon.Tests.Services
{
    public class CcuGatewayTests
    {
		public IConfigurationRoot Configuration { get; set; }

		public CcuGatewayTests()
	    {
		    var builder = new ConfigurationBuilder()
			    .SetBasePath(Directory.GetCurrentDirectory())
			    .AddUserSecrets(Startup.UserSecretId)
			    .AddEnvironmentVariables();

		    Configuration = builder.Build();
	    }

		[Fact]
		public async void GetControlInitial()
		{
			// arrange
			var auth = Configuration.GetSection("TestCcuAuth").Get<CcuAuth>();
			var logger = new Mock<ILogger<CcuGateway>>().Object;
			var gateway = new CcuGateway(logger);

			// act
			var result = await gateway.GetControlInitial(auth);

			// assert
			Assert.NotNull(result);
		}

		[Fact]
		public async void GetDeviceInfo()
		{
			// arrange
			var auth = Configuration.GetSection("TestCcuAuth").Get<CcuAuth>();
			var logger = new Mock<ILogger<CcuGateway>>().Object;
			var gateway = new CcuGateway(logger);

			// act
			var result = await gateway.GetDeviceInfo(auth);

			// assert
			Assert.NotNull(result);
		}

		[Fact]
		public async void GetStateAndEvents()
		{
			// arrange
			var auth = Configuration.GetSection("TestCcuAuth").Get<CcuAuth>();
			var logger = new Mock<ILogger<CcuGateway>>().Object;
			var gateway = new CcuGateway(logger);

			// act
			var result = await gateway.GetStateAndEvents(auth);

			// assert
			Assert.NotNull(result);
		}
    }
}
