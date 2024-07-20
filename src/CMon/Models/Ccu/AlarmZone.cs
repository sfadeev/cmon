namespace CMon.Models.Ccu
{
	public class AlarmZone
	{
		public int L { get; set; }

		public int RangeType { get; set; }

		public int RangeMin { get; set; }

		public int RangeMax { get; set; }

		public decimal UserMinVal { get; set; }

		public decimal UserMaxVal { get; set; }

		public int SensPwrSrc { get; set; }
	}
}