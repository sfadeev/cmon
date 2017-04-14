namespace CMon.Models.Ccu
{
	/// <summary>
	/// "SystemPoll":{"PowerVoltage":15.1,"PowerOk":1,"BatteryCharge":100,"Temperature":18},
	/// </summary>
	public class SystemPoll
	{
		public float PowerVoltage { get; set; }

		public bool PowerOk { get; set; }

		public int BatteryCharge { get; set; }

		public int Temperature { get; set; }
	}
}