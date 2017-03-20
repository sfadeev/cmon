namespace CMon.Models.Ccu
{
	public class ControlPoll
	{
		public int SignalLevel { get; set; }

		public int ModemStatus { get; set; }

		public float Balance { get; set; }

		public int[] Mode { get; set; }

		public int[] In { get; set; }

		public int[] Out { get; set; }
	}
}