namespace CMon.Models.Ccu
{
	public class SystemMain
	{
		public Power Power { get; set; }

		public SystemBattery Battery { get; set; }

		public Temperature Temperature { get; set; }

		public Tamper Tamper { get; set; }

		public Indication Indication { get; set; }

		public SystemPoll SystemPoll { get; set; }

		public Autopoll Autopoll { get; set; }
	}
}