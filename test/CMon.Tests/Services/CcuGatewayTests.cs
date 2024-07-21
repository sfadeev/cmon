using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CMon.Models.Ccu;
using CMon.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using IHttpClientFactory = CMon.Services.IHttpClientFactory;

namespace CMon.Tests.Services
{
	public class CcuGatewayTests
	{
		public CcuGatewayTests()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddUserSecrets(typeof(Program).Assembly)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		private IConfigurationRoot Configuration { get; }

		private ILogger<CcuGateway> Logger => new Mock<ILogger<CcuGateway>>().Object;

		private Auth UsrAuth => Configuration.GetSection("UsrAuth").Get<Auth>();

		private Auth AdmAuth => Configuration.GetSection("AdmAuth").Get<Auth>();
		
		private CcuSettings CcuSettings => Configuration.Get<CcuSettings>();

		[Test]
		public async Task GetDeviceInfo_WithInvalidAuth_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());
			var invalidAuth = new Auth
			{
				Username = "user",
				Imei = "1234567890123456",
				Password = "manager"
			};

			// act
			var result = await gateway.GetDeviceInfo(invalidAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpStatusCode);
		}

		[Test]
		public async Task GetIndexInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetIndexInitial(AdmAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);

			Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
		}

		[Test]
		public async Task GetControlInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetControlInitial(AdmAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetControlInitial_AsUser_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetControlInitial(UsrAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.InsufficientRights, result.Status.Code);
		}

		[Test]
		public async Task GetControlPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetControlPoll(AdmAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);

			var inArray = result.ControlPoll.ConvertInArray();
			
			Assert.AreEqual(result.ControlPoll.In.GetLength(0), inArray.Length);
		}

		[Test]
		public async Task GetControlPoll_AsUser_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetControlPoll(UsrAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.InsufficientRights, result.Status.Code);
		}

		[Test]
		public async Task GetInputsInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetInputsInitial(AdmAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetInputsPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetInputsPoll(AdmAuth, 0);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetInputsInputNum_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetInputsInputNum(AdmAuth, 0);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetProfilesInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetProfilesInitial(AdmAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetProfilesProfNum_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetProfilesProfNum(AdmAuth, 0);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetSystemInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetSystemInitial(AdmAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetSystemPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetSystemPoll(AdmAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetDeviceInfo()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetDeviceInfo(UsrAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test]
		public async Task GetStateAndEvents()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), new DefaultHttpClientFactory());

			// act
			var result = await gateway.GetStateAndEvents(UsrAuth);

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}

		[Test, Ignore("Legacy test, fix if needed")]
		public async Task GetStateAndEvents_FromFile()
		{
			// arrange
			var httpResponseMock = new Mock<IHttpResponse>();
			httpResponseMock.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
			httpResponseMock.Setup(x => x.ReadContentAsync(CancellationToken.None)).Returns(() => Task.FromResult(File.ReadAllText("../../../Data/GetStateAndEvents-1.json")));

			var httpClientMock = new Mock<IHttpClient>();
			httpClientMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(httpResponseMock.Object));

			var httpClientFactoryMock = new Mock<IHttpClientFactory>();
			httpClientFactoryMock.Setup(x => x.CreateClient()).Returns(() => httpClientMock.Object);

			var gateway = new CcuGateway(Logger, Options.Create(CcuSettings), httpClientFactoryMock.Object);

			// act
			var result = await gateway.GetStateAndEvents(new Auth { Imei = "1234567890", Username = "manager", Password = "secret" });

			// assert
			Assert.AreEqual(HttpStatusCode.OK, result.HttpStatusCode);
			Assert.AreEqual(StatusCode.Ok, result.Status.Code);
		}
	}
}
