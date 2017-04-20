using System.Threading.Tasks;
using CMon.Requests;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class RefreshDeviceRequestHandler : IAsyncRequestHandler<RefreshDevice>
	{
		private readonly IDeviceManagerFactory _deviceManagerFactory;

		public RefreshDeviceRequestHandler(IDeviceManagerFactory deviceManagerFactory)
		{
			_deviceManagerFactory = deviceManagerFactory;
		}

		public async Task Handle(RefreshDevice message)
		{
			var manager = _deviceManagerFactory.GetDeviceManager(message.DeviceId);

			await manager.Refresh(message);
		}
	}
}