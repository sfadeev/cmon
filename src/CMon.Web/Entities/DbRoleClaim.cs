using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace CMon.Web.Entities
{
	[Table(Schema = "public", Name = "role_claim")]
	public class DbRoleClaim : IdentityRoleClaim<long>
	{
		[Column("id"), PrimaryKey(Order = 0)]
		public override int Id { get; set; }

		[Column("role_id"), PrimaryKey(Order = 1)]
		public override long RoleId { get; set; }

		[Column("claim_type"), NotNull]
		public override string ClaimType { get; set; }

		[Column("claim_value"), NotNull]
		public override string ClaimValue { get; set; }
	}
}