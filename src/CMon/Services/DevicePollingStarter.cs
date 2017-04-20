using Microsoft.Extensions.Logging;

namespace CMon.Services
{
	public class DevicePollingStarter : IStartable
	{
		private readonly ILogger<DevicePollingStarter> _logger;
		private readonly IDeviceRepository _repository;
		private readonly IDeviceManagerFactory _deviceManagerFactory;


		public DevicePollingStarter(ILogger<DevicePollingStarter> logger, IDeviceRepository repository, IDeviceManagerFactory deviceManagerFactory)
		{
			_logger = logger;
			_repository = repository;
			_deviceManagerFactory = deviceManagerFactory;
		}

		// todo: subscribe on events of add/remove devices
		public void Start()
		{
			foreach (var device in _repository.GetDevices())
			{
				_deviceManagerFactory.GetDeviceManager(device.Id).StartPolling();
			}
		}

		public void Stop()
		{
			foreach (var device in _repository.GetDevices())
			{
				_deviceManagerFactory.GetDeviceManager(device.Id).StopPolling();
			}
		}
	}
}