using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "contract_user")]
	public class DbContractUser // : DbModificationData
	{
		[Column("contact_id"), PrimaryKey(0), NotNull]
		public long ContractId { get; set; }

		[Column("user_name"), PrimaryKey(1), NotNull]
		public string UserName { get; set; }

		[Column(Name = "role", Length = 16)]
		public string Role { get; set; }
	}
}