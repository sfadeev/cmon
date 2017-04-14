namespace CMon.Models.Ccu
{
	public class SystemInitialResult : CommandResult
	{
		public CurrentUser CurrentUser { get; set; }

		public DeviceInitial DeviceInitial { get; set; }

		public SystemInitial SystemInitial { get; set; }

		public SystemMain SystemMain { get; set; }
	}
}