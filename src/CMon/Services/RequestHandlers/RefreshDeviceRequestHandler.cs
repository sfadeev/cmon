using System;
using System.Linq;
using System.Threading.Tasks;
using CMon.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMon.Services.RequestHandlers
{
	public class RefreshDeviceRequestHandler : IAsyncRequestHandler<RefreshDevice>
	{
		private readonly ILogger<RefreshDeviceRequestHandler> _logger;
		private readonly IServiceProvider _services;

		public RefreshDeviceRequestHandler(
			ILogger<RefreshDeviceRequestHandler> logger,
			IServiceProvider services)
		{
			_logger = logger;
			_services = services;
		}

		public async Task Handle(RefreshDevice message)
		{
			var poller = _services.GetServices<IStartable>().Where(x => x is DevicePoller).Cast<DevicePoller>().Single();

			var manager = poller.GetManager(message.DeviceId);

			await manager.Refresh(message);
		}
	}
}