using Montr.Core;

namespace CMon.Commands
{
	public class AddDevice : ICommand<long>
	{
		public string Name { get; set; }

		public string Imei { get; set; }

		public string Username { get; set; }

		public string Password { get; set; }
	}
}
