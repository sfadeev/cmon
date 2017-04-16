using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

			var stateAndEvents = await _gateway.GetStateAndEvents(device.Auth);

			if (stateAndEvents != null && stateAndEvents.HttpStatusCode == HttpStatusCode.OK)
			{
				var t = (decimal) stateAndEvents.Temp;

				_repository.SaveInputValue(device.Id, BoardTemp, t);

				var message = $"[{device.Id}] - {BoardTemp}:{t:N4}";

				if (device.Config?.Inputs != null)
				{
					foreach (var input in device.Config.Inputs)
					{
						if (input.Type == InputType.Rtd02 || input.Type == InputType.Rtd03)
						{
							t = GetInputTemperature(stateAndEvents.Inputs[input.InputNo - 1].Voltage);

							_repository.SaveInputValue(device.Id, input.InputNo, t);

							message += $" - {input.InputNo}:{t:N4}";
						}
					}
				}

				if (stateAndEvents.Events != null)
				{
					var events = new List<DeviceEvent>();

					foreach (var @event in stateAndEvents.Events)
					{
						events.Add(new DeviceEvent
						{
							EventType = @event.Type,
							ExternalId = @event.Id,
							Info = new DeviceEventInformation
							{
								Number = @event.Number,
								Partitions = @event.Partitions,
								Partition = @event.Partition,
								Source = @event.Source,
								UserName = @event.UserName,
								ErrorCode = @event.ErrorCode
							}
						});
					}

					var saved = _repository.SaveEvents(device.Id, events);

					var savedIds = saved
						.Select(x => x.ExternalId)
						.Where(x => x.HasValue)
						.Cast<long>()
						.ToArray();

					if (savedIds.Length > 0)
					{
						await _gateway.AckEvents(device.Auth, savedIds);
					}
				}

				_logger.LogInformation(message);
			}
		}

		private static decimal GetInputTemperature(long discrete)
		{
			var voltage = discrete * 10M / 4095;

			var temp = (voltage / 5M - 0.5M) / 0.01M;

			return temp;
		}
	}
}