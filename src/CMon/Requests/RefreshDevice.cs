using MediatR;

namespace CMon.Requests
{
	public class RefreshDevice : IRequest<bool>
	{
		public string UserName { get; set; }

		public long DeviceId { get; set; }
	}
}