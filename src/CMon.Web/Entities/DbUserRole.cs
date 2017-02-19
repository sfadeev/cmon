using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace CMon.Web.Entities
{
	[Table(Schema = "public", Name = "user_role")]
	public class DbUserRole : IdentityUserRole<long>
	{
		[Column("user_id"), PrimaryKey(Order = 0)]
		public override long UserId { get; set; }

		[Column("role_id"), PrimaryKey(Order = 1)]
		public override long RoleId { get; set; }
	}
}