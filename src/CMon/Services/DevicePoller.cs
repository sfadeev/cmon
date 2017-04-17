using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMon.Services
{
	public class DevicePoller : IStartable
	{
		private readonly ILogger<DevicePoller> _logger;
		private readonly IServiceProvider _services;
		private readonly IDeviceRepository _repository;

		private readonly ConcurrentDictionary<long, IDeviceManager> _deviceManagers = new ConcurrentDictionary<long, IDeviceManager>();
		private readonly ConcurrentDictionary<long, Timer> _timers = new ConcurrentDictionary<long, Timer>();

		public DevicePoller(ILogger<DevicePoller> logger, IServiceProvider services, IDeviceRepository repository)
		{
			_logger = logger;
			_services = services;
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
						deviceManager.PollAsync(deviceId).Wait();
					}
					catch (Exception ex)
					{
						_logger.LogError(0, ex, "Error sending request for device id {deviceId}", deviceId);
					}
				}, deviceId, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(25));
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
	}
}