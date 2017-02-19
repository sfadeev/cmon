using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace CMon.Web.Entities
{
	[Table(Schema = "public", Name = "user_token")]
	public class DbUserToken : IdentityUserToken<long>
	{
		[Column("user_id"), PrimaryKey(Order = 0)]
		public override long UserId { get; set; }

		[Column("login_provider"), PrimaryKey(Order = 1)]
		public override string LoginProvider { get; set; }

		[Column("name"), PrimaryKey(Order = 2)]
		public override string Name { get; set; }

		[Column("value")]
		public override string Value { get; set; }
	}
}