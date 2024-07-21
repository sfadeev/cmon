namespace CMon.Models.Ccu
{
	public class ControlInitialResult : CommandResult
	{
		public TopLine TopLine { get; set; }

		public DeviceInitial DeviceInitial { get; set; }

		public ControlInitial ControlInitial { get; set; }

		public string[] InputsSchema { get; set; }
		
		public string[] OutputsSchema { get; set; }
		
		public ControlPoll ControlPoll { get; set; }

		public Autopoll Autopoll { get; set; }
	}
}