using System;
using LinqToDB.Mapping;

namespace CMon.Entities
{
	public abstract class DbModificationData
	{
		[Column("created_at"), NotNull]
		public DateTime? CreatedAt { get; set; }

		[Column("created_by")]
		public string CreatedBy { get; set; }

		[Column("modified_at")]
		public DateTime? ModifiedAt { get; set; }

		[Column("modified_by")]
		public string ModifiedBy { get; set; }
	}
}