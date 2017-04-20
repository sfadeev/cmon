using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace CMon.Services
{
	public interface IDeviceManagerFactory
	{
		IDeviceManager GetDeviceManager(long deviceId);
	}

	public class DefaultDeviceManagerFactory : IDeviceManagerFactory
	{
		private readonly IServiceProvider _services;

		private readonly ConcurrentDictionary<long, IDeviceManager> _deviceManagers = new ConcurrentDictionary<long, IDeviceManager>();

		public DefaultDeviceManagerFactory(IServiceProvider services)
		{
			_services = services;
		}

		public IDeviceManager GetDeviceManager(long deviceId)
		{
			return _deviceManagers.GetOrAdd(deviceId, x =>
			{
				var deviceManager = _services.GetService<CcuDeviceManager>();
				deviceManager.Configure(deviceId);
				return deviceManager;
			});
		}
	}
}