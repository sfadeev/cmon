using System.IO;
using System.Net;
using CMon.Models.Ccu;
using CMon.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CMon.Tests.Services
{
	public class CcuGatewayTests
	{
		private readonly ITestOutputHelper _output;

		public IConfigurationRoot Configuration { get; set; }

		public CcuGatewayTests(ITestOutputHelper output)
		{
			_output = output;

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddUserSecrets(Startup.UserSecretId)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		private ILogger<CcuGateway> Logger => new Mock<ILogger<CcuGateway>>().Object;

		private Auth UsrAuth => Configuration.GetSection("UsrAuth").Get<Auth>();

		private Auth AdmAuth => Configuration.GetSection("AdmAuth").Get<Auth>();

		[Fact]
		public async void GetDeviceInfo_WithInvalidAuth_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger);
			var invalidAuth = new Auth
			{
				Username = "user",
				Imei = "1234567890123456",
				Password = "manager"
			};

			// act
			var result = await gateway.GetDeviceInfo(invalidAuth);

			// assert
			Assert.Equal(HttpStatusCode.BadRequest, result.HttpStatusCode);
		}

		[Fact]
		public async void GetIndexInitial()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetIndexInitial(AdmAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);

			_output.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
		}

		[Fact]
		public async void GetControlInitial()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetControlInitial(AdmAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetControlInitial_AsUser_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetControlInitial(UsrAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Equal(StatusCode.InsufficientRights, result.Status.Code);
		}

		[Fact]
		public async void GetControlPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetControlPoll(AdmAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetControlPoll_AsUser_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetControlPoll(UsrAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Equal(StatusCode.InsufficientRights, result.Status.Code);
		}

		[Fact]
		public async void GetInputsInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetInputsInitial(AdmAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetInputsPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetInputsPoll(AdmAuth, 0);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetInputsInputNum_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetInputsInputNum(AdmAuth, 0);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetProfilesInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetProfilesInitial(AdmAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetProfilesProfNum_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetProfilesProfNum(AdmAuth, 0);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetSystemInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetSystemInitial(AdmAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetSystemPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetSystemPoll(AdmAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetDeviceInfo()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetDeviceInfo(UsrAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Null(result.Status);
		}

		[Fact]
		public async void GetStateAndEvents()
		{
			// arrange
			var gateway = new CcuGateway(Logger);

			// act
			var result = await gateway.GetStateAndEvents(UsrAuth);

			// assert
			Assert.Equal(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.Equal(StatusCode.Ok, result.Status.Code);
		}
	}
}
