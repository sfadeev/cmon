namespace CMon.Models.Ccu
{
	public class ControlInitialResult : CommandResult
	{
		public CurrentUser CurrentUser { get; set; }

		public DeviceInitial DeviceInitial { get; set; }

		public ControlInitial ControlInitial { get; set; }

		public ControlPoll ControlPoll { get; set; }

		public Autopoll Autopoll { get; set; }
	}
}