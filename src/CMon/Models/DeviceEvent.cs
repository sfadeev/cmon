namespace CMon.Models
{
	public class DeviceEvent : ModificationData
	{
		public long Id { get; set; }

		public string EventType { get; set; }

		public long? ExternalId { get; set; }

		public DeviceEventInformation Info { get; set; }

		public string DisplayTitle { get; set; }

		public string DisplayIcon { get; set; }

		public string[] DisplayParams { get; set; }
	}
}