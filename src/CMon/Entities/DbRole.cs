using System;
using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "roles")]
	public class DbRole : IdentityRole<long>
	{
		[Column("id"), PrimaryKey, Identity, NotNull]
		public override long Id { get; set; }

		[Column("name")]
		public override string Name { get; set; }

		[Column("normalized_name")]
		public override string NormalizedName { get; set; }

		[Column("concurrency_stamp"), NotNull]
		public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
	}
}