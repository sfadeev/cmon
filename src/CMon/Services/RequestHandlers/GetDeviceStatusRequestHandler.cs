using System;
using System.Linq;
using System.Threading.Tasks;
using CMon.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CMon.Services.RequestHandlers
{
	public class GetDeviceStatusRequestHandler : IAsyncRequestHandler<GetDeviceStatus, string>
	{
		private readonly ILogger<GetDeviceStatusRequestHandler> _logger;
		private readonly IServiceProvider _services;

		public GetDeviceStatusRequestHandler(
			ILogger<GetDeviceStatusRequestHandler> logger,
			IServiceProvider services)
		{
			_logger = logger;
			_services = services;
		}

		public Task<string> Handle(GetDeviceStatus message)
		{
			var poller = _services.GetServices<IStartable>().Where(x => x is DevicePoller).Cast<DevicePoller>().Single();

			var manager = poller.GetManager(message.DeviceId);

			var result = manager.GetStatus();

			return Task.FromResult(result);
		}
	}
}