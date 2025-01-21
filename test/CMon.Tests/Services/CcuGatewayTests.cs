using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CMon.Models.Ccu;
using CMon.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CMon.Tests.Services
{
	public class CcuGatewayTests
	{
		public CcuGatewayTests()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				// ccu passwords for tests stored in user secrets
				// todo: move to env
				.AddUserSecrets(typeof(Program).Assembly)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		private IConfigurationRoot Configuration { get; }

		private ILogger<CcuGateway> Logger => new Mock<ILogger<CcuGateway>>().Object;

		private Auth UsrAuth => Configuration.GetSection("UsrAuth").Get<Auth>();

		private Auth AdmAuth => Configuration.GetSection("AdmAuth").Get<Auth>();
		
		private AppOptions AppOptions => Configuration.Get<AppOptions>();

		[Test]
		public async Task GetDeviceInfo_WithInvalidAuth_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());
			var invalidAuth = new Auth
			{
				Username = "user",
				Imei = "1234567890123456",
				Password = "manager"
			};

			// act
			var result = await gateway.GetDeviceInfo(invalidAuth);

			// assert
			Assert.That(HttpStatusCode.BadRequest, Is.EqualTo(result.HttpStatusCode));
		}
		
		[Test]
		public async Task GetIndexInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetIndexInitial(AdmAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));

			Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
		}

		[Test]
		public async Task GetControlInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetControlInitial(AdmAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetControlInitial_AsUser_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetControlInitial(UsrAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.InsufficientRights, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetControlPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetControlPoll(AdmAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));

			var inArray = result.ControlPoll.ConvertInArray();
			
			Assert.That(result.ControlPoll.In.GetLength(0), Is.EqualTo(inArray.Length));
		}

		[Test]
		public async Task GetControlPoll_AsUser_ShouldReturnError()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetControlPoll(UsrAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.InsufficientRights, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetInputsInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetInputsInitial(AdmAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetInputsPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetInputsPoll(AdmAuth, 0);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetInputsInputNum_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetInputsInputNum(AdmAuth, 0);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetProfilesInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetProfilesInitial(AdmAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetProfilesProfNum_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetProfilesProfNum(AdmAuth, 0);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetSystemInitial_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetSystemInitial(AdmAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetSystemPoll_AsAdmin()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetSystemPoll(AdmAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetDeviceInfo()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetDeviceInfo(UsrAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}

		[Test]
		public async Task GetStateAndEvents()
		{
			// arrange
			var gateway = new CcuGateway(Logger, Options.Create(AppOptions), CreateHttpClientFactoryMock());

			// act
			var result = await gateway.GetStateAndEvents(UsrAuth);

			// assert
			Assert.That(HttpStatusCode.OK, Is.EqualTo(result.HttpStatusCode));
			Assert.That(StatusCode.Ok, Is.EqualTo(result.Status.Code));
		}
		
		private static IHttpClientFactory CreateHttpClientFactoryMock()
		{
			var mockHttpClientFactory = new Mock<IHttpClientFactory>();
			mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient());
			var httpClientFactory = mockHttpClientFactory.Object;
			return httpClientFactory;
		}

	}
}
