using System;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "data_protection_key")]
	public class DbDataProtectionKey 
	{
		[Column("id"), PrimaryKey, NotNull]
		public string Id { get; set; }

		[Column("data"), NotNull]
		public string Data { get; set; }

		[Column(Name = "created_at"), NotNull]
		public DateTime CreatedAt { get; set; }
	}
}