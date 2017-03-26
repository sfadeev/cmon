using Montr.Core;

namespace CMon.Commands
{
	public class GetDeviceStatus : ICommand<string>
	{
		public long DeviceId { get; set; }
	}
}