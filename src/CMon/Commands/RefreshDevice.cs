using Montr.Core;

namespace CMon.Commands
{
	public class RefreshDevice : ICommand<bool>
	{
		public long DeviceId { get; set; }
	}
}