namespace CMon.Models.Ccu
{
	/// <summary>
	/// "Indication":{"L":1,"FadeInpLeds":1,"IndMode":1,"ShowElapsedAct":0,"ShowActArmOut":0}
	/// </summary>
	public class Indication
	{
		public int L { get; set; }

		public bool FadeInpLeds { get; set; }

		public IndicationMode IndMode { get; set; }

		public bool ShowElapsedAct { get; set; }

		public bool ShowActArmOut { get; set; }
	}
}