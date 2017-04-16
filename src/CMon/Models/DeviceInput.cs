namespace CMon.Models
{
	public class DeviceInput
	{
		public short InputNo { get; set; }

		public InputType Type { get; set; }

		public string Name { get; set; }

		public string ActiveName { get; set; }

		public string PassiveName { get; set; }

		public RangeType AlarmZoneRangeType { get; set; }

		public decimal? AlarmZoneMinValue { get; set; }

		public decimal? AlarmZoneMaxValue { get; set; }
	}
}