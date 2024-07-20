namespace CMon.Models.Ccu
{
	public class SystemBattery
	{
		public int L { get; set; }

		public int Level1 { get; set; }

		public int Level2 { get; set; }

		public bool Level1React { get; set; }

		public int Level1Event { get; set; }

		public int Level1ReactOut { get; set; }

		public bool Level2React { get; set; }

		public int Level2ReactOut { get; set; }

		public int Level2Event { get; set; }
	}
}