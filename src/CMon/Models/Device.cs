using CMon.Models.Ccu;

namespace CMon.Models
{
	public class Device : ModificationData
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Imei { get; set; }

		public Auth Auth { get; set; }

		public DeviceConfig Config { get; set; }

		public byte[] Hash { get; set; }
	}
}