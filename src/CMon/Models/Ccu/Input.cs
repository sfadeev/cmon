using System.Diagnostics;

namespace CMon.Models.Ccu
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class Input
	{
		public byte Active { get; set; }

		public int Voltage { get; set; }

		private string DebuggerDisplay => $"Active: {Active}, Voltage: {Voltage}";
	}
}