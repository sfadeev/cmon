namespace CMon.Models.Ccu
{
	public class SystemInitialResult : CommandResult
	{
		public TopLine TopLine { get; set; }

		public DeviceInitial DeviceInitial { get; set; }

		public SystemInitial SystemInitial { get; set; }

		public SystemMain SystemMain { get; set; }
	}
}