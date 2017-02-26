using System;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "input_value")]
	public class DbInputValue
	{
		[Column(Name = "device_id"), NotNull]
		public long DeviceId { get; set; }

		[Column(Name = "input_no"), NotNull]
		public short InputNum { get; set; }

		[Column(Name = "value"), NotNull]
		public decimal Value { get; set; }

		[Column(Name = "created_at"), NotNull]
		public DateTime CreatedAt { get; set; }
	}
}
