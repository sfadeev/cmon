using System;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	[Table(Schema = "public", Name = "contract_operation")]
	public class DbContractOperation
	{
		[Column("id"), PrimaryKey, Identity, NotNull]
		public long Id { get; set; }

		[Column("contact_id"), NotNull]
		public long ContractId { get; set; }

		[Column("created_at"), NotNull]
		public DateTime? CreatedAt { get; set; }

		[Column("created_by")]
		public string CreatedBy { get; set; }
	}
}