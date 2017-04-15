using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CMon.Services
{
	public class DevicePoller : IStartable
	{
		public const short BoardTemp = 0xFF;

		private readonly ILogger<DevicePoller> _logger;
		private readonly IServiceProvider _services;
		private readonly ICcuGateway _gateway;
		private readonly IDeviceRepository _repository;

		private readonly ConcurrentDictionary<long, IDeviceManager> _deviceManagers = new ConcurrentDictionary<long, IDeviceManager>();
		private readonly ConcurrentDictionary<long, Timer> _timers = new ConcurrentDictionary<long, Timer>();

		public DevicePoller(ILogger<DevicePoller> logger, IServiceProvider services, ICcuGateway gateway, IDeviceRepository repository)
		{
			_logger = logger;
			_services = services;
			_gateway = gateway;
			_repository = repository;
		}

		public IDeviceManager GetManager(long deviceId)
		{
			return _deviceManagers[deviceId];
		}

		public void Start()
		{
			foreach (var deviceId in _repository.GetDevices().Select(x => x.Id))
			{
				// todo: subscribe on events of add/remove devices
				var deviceManager = _deviceManagers[deviceId] = _services.GetService<CcuDeviceManager>();

				deviceManager.Configure(deviceId);

				_timers[deviceId] = new Timer(state =>
				{
					try
					{
						PollAsync(deviceId).Wait();
					}
					catch (Exception ex)
					{
						_logger.LogError(0, ex, "Error sending request for device id {deviceId}", deviceId);
					}
				}, deviceId, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15));
			}
		}

		public void Stop()
		{
			foreach (var timer in _timers.Values)
			{
				timer.Dispose();
			}

			_timers.Clear();
		}

		public async Task PollAsync(long deviceId)
		{
			var device = _repository.GetDevice(deviceId);

			/*var auth = new Auth { Imei = device.Imei, Username = device.Username, Password = device.Password };

			var stateAndEvents = _gateway.GetStateAndEvents(auth);*/

			var inputs = _repository.GetInputs(deviceId);

			var url = "https://ccu.sh/data.cgx?cmd={\"Command\":\"GetStateAndEvents\"}";

			var json = await Get(device, url);

			if (json != null)
			{
				var jo = JObject.Parse(json);

				// if (jo.SelectToken("Events")?.HasValues == true)
				if (/*device.Id == 1 &&*/ json.Length >= 609)
				{
					_logger.LogInformation(url + "\n" + json);

					// File.WriteAllText("c:\\temp\\ccu\\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-GetStateAndEvents.json", json);
				}

				var t = GetBoardTemperature(jo);
				_repository.SaveToDb(device.Id, BoardTemp, t);

				var message = $"[{device.Id}] - {BoardTemp}:{t:N4}";

				foreach (var input in inputs)
				{
					t = GetInputTemperature(jo, input.InputNo - 1);
					_repository.SaveToDb(device.Id, input.InputNo, t);

					message += $" - {input.InputNo}:{t:N4}";
				}

				_logger.LogInformation(message);
			}
		}

		private async Task<string> Get(DbDevice device, string url)
		{
			using (var client = new HttpClient())
			{
				try
				{
					var auth = $"{device.Username}@{device.Imei}:{device.Password}";
					var authBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(auth));

					client.DefaultRequestHeaders.Add("Authorization", "Basic " + authBase64);

					var response = await client.GetAsync(url);

					// response.EnsureSuccessStatusCode();

					if (response.StatusCode != HttpStatusCode.OK)
					{
						_logger.LogError("[{0}] - {1} {2} - {3}", device.Id, (int)response.StatusCode, response.StatusCode, response.ReasonPhrase);
						return null;
					}

					return await response.Content.ReadAsStringAsync();
				}
				catch (HttpRequestException ex)
				{
					_logger.LogError(0, ex, "Get(url) exception");
				}
			}

			return null;
		}

		private static decimal GetInputTemperature(JObject jo, int input)
		{
			var discrete = jo.SelectToken($"Inputs[{input}].Voltage").Value<long>();
			var voltage = discrete * 10M / 4095;
			var temp = (voltage / 5M - 0.5M) / 0.01M;

			return temp;
		}

		private static decimal GetBoardTemperature(JObject jo)
		{
			var temp = jo.SelectToken("Temp").Value<long>();

			return temp;
		}
	}
}