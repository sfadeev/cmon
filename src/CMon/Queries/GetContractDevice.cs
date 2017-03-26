using CMon.Models;
using Montr.Core;

namespace CMon.Queries
{
	public class GetContractDevice : IQuery<Device>
	{
		public long DeviceId { get; set; }

		public bool WithAuth { get; set; }
	}
}