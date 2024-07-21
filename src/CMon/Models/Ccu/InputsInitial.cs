namespace CMon.Models.Ccu
{
	public class InputsInitial
	{
		public int ScenCount { get; set; }

		public int InCount { get; set; }

		public int OutCount { get; set; }

		public int RelayCount { get; set; }

		public int[] Rtd04Limits { get; set; }
		
		public int[] Rtd05Limits { get; set; }
	}
}