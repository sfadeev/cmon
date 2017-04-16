using System;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "event")]
	public class DbEvent
	{
		[Column("id"), PrimaryKey, Identity, NotNull]
		public long Id { get; set; }

		[Column("device_id"), NotNull]
		public long DeviceId { get; set; }

		[Column("created_at"), NotNull]
		public DateTime? CreatedAt { get; set; }

		[Column("created_by")]
		public string CreatedBy { get; set; }

		[Column("event_type"), NotNull]
		public string EventType { get; set; }

		[Column("external_id")]
		public long? ExternalId { get; set; }

		[Column("info")]
		public string Info { get; set; }
	}
}