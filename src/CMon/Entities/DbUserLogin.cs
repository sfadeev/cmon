using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "user_login")]
	public class DbUserLogin
	{
		[Column("user_id"), PrimaryKey(Order = 0)]
		public long UserId { get; set; }

		[Column("login_provider"), PrimaryKey(Order = 1)]
		public string LoginProvider { get; set; }

		[Column("provider_key"), NotNull]
		public string ProviderKey { get; set; }
	}
}