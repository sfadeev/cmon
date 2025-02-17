using System.Diagnostics;

namespace CMon.Models.Ccu
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class Battery
	{
		public BatteryState State { get; set; }

		public int? Charge { get; set; }

		private string DebuggerDisplay => $"State: {State}, Charge: {Charge}";
	}
}