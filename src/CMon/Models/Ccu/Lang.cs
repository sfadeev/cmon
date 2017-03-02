using System.Diagnostics;

namespace CMon.Models.Ccu
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class Lang
	{
		public int Index { get; set; }

		public string Name { get; set; }

		private string DebuggerDisplay => $"Index: {Index}, Name: {Name}";
	}
}