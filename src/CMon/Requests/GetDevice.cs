using CMon.Models;
using MediatR;

namespace CMon.Requests
{
	public class GetDevice : IRequest<Device>
	{
		public string UserName { get; set; }

		public long DeviceId { get; set; }

		public bool WithAuth { get; set; }
	}
}