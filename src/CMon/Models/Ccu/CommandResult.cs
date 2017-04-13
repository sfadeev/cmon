using System.Net;

namespace CMon.Models.Ccu
{
	public abstract class CommandResult
	{
		public Status Status { get; set; }

		public StatusCode Code { get; set; }

		public HttpStatusCode HttpStatusCode { get; set; }
	}
}