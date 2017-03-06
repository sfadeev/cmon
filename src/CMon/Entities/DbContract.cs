using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "contract")]
	public class DbContract : DbModificationData
	{
		[Column("id"), PrimaryKey, Identity, NotNull]
		public long Id { get; set; }
	}
}