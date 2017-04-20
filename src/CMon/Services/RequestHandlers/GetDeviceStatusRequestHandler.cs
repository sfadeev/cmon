using System.Threading.Tasks;
using CMon.Requests;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class GetDeviceStatusRequestHandler : IAsyncRequestHandler<GetDeviceStatus, string>
	{
		private readonly IDeviceManagerFactory _deviceManagerFactory;

		public GetDeviceStatusRequestHandler(IDeviceManagerFactory deviceManagerFactory)
		{
			_deviceManagerFactory = deviceManagerFactory;
		}

		public Task<string> Handle(GetDeviceStatus message)
		{
			var manager = _deviceManagerFactory.GetDeviceManager(message.DeviceId);

			var result = manager.GetStatus();

			return Task.FromResult(result);
		}
	}
}