namespace CMon.Models.Ccu
{
	public class ProfilesProfNum
	{
		public int L { get; set; }

		public int ProfNum { get; set; }

		public bool Enable { get; set; }

		public string ProfName { get; set; }

		public ProfileNumInput[] Inputs { get; set; }

		public int[] OutsReact { get; set; }
	}
}