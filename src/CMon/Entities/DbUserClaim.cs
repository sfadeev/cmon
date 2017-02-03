using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "user_claim")]
	public class DbUserClaim
	{
		[Column("user_id"), PrimaryKey(Order = 0)]
		public long UserId { get; set; }

		[Column("claim_type_id"), PrimaryKey(Order = 1)]
		public long ClaimTypeId { get; set; }

		[Column("value"), NotNull]
		public string Value { get; set; }
	}
}