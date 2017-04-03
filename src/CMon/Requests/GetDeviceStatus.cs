using MediatR;

namespace CMon.Requests
{
	public class GetDeviceStatus : IRequest<string>
	{
		public long DeviceId { get; set; }
	}
}