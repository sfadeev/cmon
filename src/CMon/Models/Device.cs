using CMon.Models.Ccu;

namespace CMon.Models
{
	public class Device
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Imei { get; set; }

		public string Hash { get; set; }

		public Auth Auth { get; set; }
	}
}