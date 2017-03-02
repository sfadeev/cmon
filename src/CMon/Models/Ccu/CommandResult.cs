using System.Net;

namespace CMon.Models.Ccu
{
	public abstract class CommandResult
	{
		public Status Status { get; set; }

		public HttpStatusCode Code { get; set; }

		public HttpStatusCode HttpStatusCode { get; set; }
	}
}