namespace CMon.Models.Ccu
{
	public class DeviceInfo : CommandResult
	{
		public string DeviceType { get; set; }

		public string DeviceMod { get; set; }

		public string HwVer { get; set; }

		public string FwVer { get; set; }

		public string BootVer { get; set; }

		public string FwBuildDate { get; set; }

		public string CountryCode { get; set; }

		public string Serial { get; set; }

		public string Imei { get; set; }

		public int InputsCount { get; set; }

		public int PartitionsCount { get; set; }

		public string ExtBoard { get; set; }

		public string uGuardVerCode { get; set; }
	}
}