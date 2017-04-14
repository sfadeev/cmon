namespace CMon.Models.Ccu
{
	public class Power
	{
		public int L { get; set; }

		public bool OnEvent { get; set; }

		public bool OffEvent { get; set; }

		public bool DiscardShortImp { get; set; }

		public int ControlRecoveryDelay { get; set; }

		public int OffReactOut { get; set; }

		public int OffReact { get; set; }

		public int OnReactOut { get; set; }

		public int OnReact { get; set; }
	}
}