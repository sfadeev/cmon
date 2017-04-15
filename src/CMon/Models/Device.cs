using System;
using CMon.Models.Ccu;

namespace CMon.Models
{
	public class Device : ModificationData
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public string Imei { get; set; }

		public Auth Auth { get; set; }

		public DeviceConfig Config { get; set; }

		public byte[] Hash { get; set; }
	}

	public abstract class ModificationData
	{
		public DateTime? CreatedAt { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? ModifiedAt { get; set; }

		public string ModifiedBy { get; set; }
	}

	public class DeviceConfig
	{
		public DeviceInformation Info { get; set; }

		public DeviceInput[] Inputs { get; set; }
	}

	public class DeviceInformation
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

	public class DeviceInput
	{
		public short InputNo { get; set; }

		public InputType Type { get; set; }

		public string Name { get; set; }

		public string ActiveName { get; set; }

		public string PassiveName { get; set; }

		public RangeType AlarmZoneRangeType { get; set; }

		public decimal? AlarmZoneMinValue { get; set; }

		public decimal? AlarmZoneMaxValue { get; set; }
	}
}