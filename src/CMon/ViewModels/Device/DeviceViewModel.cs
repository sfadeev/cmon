using System.Collections.Generic;

namespace CMon.ViewModels.Device
{
	public class DeviceListViewModel
	{
		public IList<DeviceViewModel> Items { get; set; }
	}
	
	public class DevicePageViewModel
	{
		public long? Id { get; set; }

		public IList<DeviceViewModel> Devices { get; set; }

		public DateRange[][] QuickRanges { get; set; }
	}
	
	public class DeviceViewModel
	{
		public string Url { get; set; }

		public long Id { get; set; }

		public string Name { get; set; }

		public string Imei { get; set; }
	}
	
	public class BlockViewModel
	{
		public string Type { get; set; }

		public string Name { get; set; }

		public short InputNo { get; set; }
	}
	
	public class DateRange
	{
		public string Name { get; set; }

		public string From { get; set; }

		public string To { get; set; }
	}
}