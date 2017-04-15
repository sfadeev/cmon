namespace CMon.Models
{
	public enum RangeType : byte
	{
		None = 0,
		LowOrHigh = 1,
		Low = 2,
		Average = 3,
		High = 4,
		LowHysteresis = 5,
		HighHysteresis = 6
	}
}