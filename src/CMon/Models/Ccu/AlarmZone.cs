namespace CMon.Models.Ccu
{
	public class AlarmZone
	{
		public int L { get; set; }

		public int RangeType { get; set; }

		public int RangeMin { get; set; }

		public int RangeMax { get; set; }

		public float UserMinVal { get; set; }

		public float UserMaxVal { get; set; }

		public int SensPwrSrc { get; set; }
	}
}