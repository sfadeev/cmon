using System.Diagnostics;

namespace CMon.Models.Ccu
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class ProfileInput
	{
		public int InputType { get; set; }

		public int RangeType { get; set; }

		public float UserMinVal { get; set; }

		public float UserMaxVal { get; set; }

		private string DebuggerDisplay => $"InputType: {InputType}, RangeType: {RangeType}, UserMinVal: {UserMinVal}, UserMaxVal: {UserMaxVal}";
	}
}