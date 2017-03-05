using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "device")]
	public class DbDevice
	{
		[Column("id"), PrimaryKey, Identity, NotNull]
		public long Id { get; set; }

		[Column(Name = "imei")]
		public string Imei { get; set; }

		[Column(Name = "username")]
		public string Username { get; set; }

		[Column(Name = "password")]
		public string Password { get; set; }
	}
}