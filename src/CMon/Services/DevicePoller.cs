using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Models;
using CMon.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMon.Services
{
	public class DevicePoller : IStartable
	{
		public const short BoardTemp = 0xFF;

		private readonly ILogger<DevicePoller> _logger;
		private readonly IMediator _mediator;
		private readonly IServiceProvider _services;
		private readonly ICcuGateway _gateway;
		private readonly IDeviceRepository _repository;

		private readonly ConcurrentDictionary<long, IDeviceManager> _deviceManagers = new ConcurrentDictionary<long, IDeviceManager>();
		private readonly ConcurrentDictionary<long, Timer> _timers = new ConcurrentDictionary<long, Timer>();

		public DevicePoller(ILogger<DevicePoller> logger,
			IMediator mediator, IServiceProvider services, ICcuGateway gateway, IDeviceRepository repository)
		{
			_logger = logger;
			_mediator = mediator;
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
			var device = await _mediator.Send(
				new GetDevice { DeviceId = deviceId, WithAuth = true });

			var stateAndEvents = _gateway.GetStateAndEvents(device.Auth);

			var t = (decimal) stateAndEvents.Result.Temp;

			_repository.SaveToDb(device.Id, BoardTemp, t);

			var message = $"[{device.Id}] - {BoardTemp}:{t:N4}";

			if (device.Config?.Inputs != null)
			{
				foreach (var input in device.Config.Inputs)
				{
					if (input.Type == InputType.Rtd02 || input.Type == InputType.Rtd03)
					{
						t = GetInputTemperature(stateAndEvents.Result.Inputs[input.InputNo - 1].Voltage);

						_repository.SaveToDb(device.Id, input.InputNo, t);

						message += $" - {input.InputNo}:{t:N4}";
					}
				}
			}

			// todo: write messages to operations log

			_logger.LogInformation(message);
		}

		private static decimal GetInputTemperature(long discrete)
		{
			var voltage = discrete * 10M / 4095;

			var temp = (voltage / 5M - 0.5M) / 0.01M;

			return temp;
		}
	}
}