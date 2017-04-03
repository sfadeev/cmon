using MediatR;

namespace CMon.Requests
{
	public class AddDevice : IRequest<long>
	{
		public string Name { get; set; }

		public string Imei { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }
	}
}
