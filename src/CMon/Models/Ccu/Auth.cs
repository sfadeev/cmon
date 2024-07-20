using System.Diagnostics;

namespace CMon.Models.Ccu
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class Auth
	{
		public string Imei { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }

		public string DebuggerDisplay => $"{Username}@{Imei}";
	}
}