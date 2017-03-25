using System.Collections.Generic;

namespace CMon.ViewModels.Device
{
	public class DeviceListViewModel
	{
		public IList<DeviceViewModel> Items { get; set; }
	}
	
	public class DeviceViewModel
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Imei { get; set; }

		public DateRange[][] QuickRanges { get; set; }
	}
	
	public class DateRange
	{
		public string Name { get; set; }

		public string From { get; set; }

		public string To { get; set; }
	}
}