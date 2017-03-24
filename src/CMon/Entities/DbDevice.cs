using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "device")]
	public class DbDevice : DbModificationData
	{
		[Column("id"), PrimaryKey, Identity, NotNull]
		public long Id { get; set; }

		[Column("contract_id")]
		public long? ContractId { get; set; }

		[Column(Name = "status"), NotNull]
		public DeviceStatus Status { get; set; }

		[Column(Name = "name")]
		public string Name { get; set; }

		[Column(Name = "imei")]
		public string Imei { get; set; }

		[Column(Name = "username")]
		public string Username { get; set; }

		[Column(Name = "password")]
		public string Password { get; set; }
	}

	public enum DeviceStatus
	{
		None = 0,
		Active = 1
	}
}