using System;

namespace CMon.Models
{
	public abstract class ModificationData
	{
		public DateTime? CreatedAt { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? ModifiedAt { get; set; }

		public string ModifiedBy { get; set; }
	}
}