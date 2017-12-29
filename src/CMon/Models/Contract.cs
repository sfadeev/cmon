using System.Collections.Generic;

namespace CMon.Models
{
	public class Contract : ModificationData
	{
		public long Id { get; set; }

		public IList<Device> Devices { get; set; }
	}
}