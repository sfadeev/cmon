using CMon.ViewModels.Device;
using MediatR;

namespace CMon.Requests
{
	public class GetDeviceList : IRequest<DeviceListViewModel>
	{
		public string UserName { get; set; }
	}
}