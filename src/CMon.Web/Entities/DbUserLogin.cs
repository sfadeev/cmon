using LinqToDB.Identity;
using LinqToDB.Mapping;

namespace CMon.Web.Entities
{
	[Table(Schema = "public", Name = "user_login")]
	public class DbUserLogin : IdentityUserLogin<long>
	{
		[Column("login_provider"), PrimaryKey(Order = 0)]
		public override string LoginProvider { get; set; }

		[Column("provider_key"), PrimaryKey(Order = 1)]
		public override string ProviderKey { get; set; }

		[Column("user_id"), NotNull]
		public override long UserId { get; set; }

		[Column("provider_display_name")]
		public override string ProviderDisplayName { get; set; }
	}
}