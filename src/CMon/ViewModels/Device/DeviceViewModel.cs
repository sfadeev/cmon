namespace CMon.ViewModels.Device
{
	public class DeviceViewModel
	{
		public long Id { get; set; }

		public DateRange[][] QuickRanges { get; set; }
	}
	
	public class DateRange
	{
		public string Name { get; set; }

		public string From { get; set; }

		public string To { get; set; }
	}
}