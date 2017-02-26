using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "device")]
	public class DbDevice
	{
		[Column(Name = "id"), NotNull, PrimaryKey]
		public long Id { get; set; }

		[Column(Name = "imei")]
		public string Imei { get; set; }

		[Column(Name = "username")]
		public string Username { get; set; }

		[Column(Name = "password")]
		public string Password { get; set; }
	}
}