using CMon.Models;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "input")]
	public class DbInput
	{
		[Column(Name = "device_id"), NotNull, PrimaryKey(0)]
		public long DeviceId { get; set; }

		[Column(Name = "input_no"), NotNull, PrimaryKey(1)]
		public short InputNo { get; set; }

		[Column(Name = "type"), NotNull]
		public InputType Type { get; set; }

		[Column(Name = "name", Length = 16)]
		public string Name { get; set; }

		[Column(Name = "active_name", Length = 16)]
		public string ActiveName { get; set; }

		[Column(Name = "passive_name", Length = 16)]
		public string PassiveName { get; set; }

		[Column(Name = "alarm_zone_range_type"), NotNull]
		public RangeType AlarmZoneRangeType { get; set; }

		[Column(Name = "alarm_zone_min_value")]
		public decimal? AlarmZoneMinValue { get; set; }

		[Column(Name = "alarm_zone_max_value")]
		public decimal? AlarmZoneMaxValue { get; set; }
	}
}