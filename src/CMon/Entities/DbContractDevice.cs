using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "contract_device")]
	public class DbContractDevice // : DbModificationData
	{
		[Column("contact_id"), PrimaryKey(0), NotNull]
		public long ContractId { get; set; }

		[Column("device_id"), PrimaryKey(1), NotNull]
		public long DeviceId { get; set; }
	}
}