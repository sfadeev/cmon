namespace CMon.Models.Ccu
{
	public class InputsInitialResult : CommandResult
	{
		public TopLine TopLine { get; set; }

		public DeviceInitial DeviceInitial { get; set; }
		
		public string[] InputsSchema { get; set; }

		public string[] OutputsSchema { get; set; }
		
		public InputsInitial InputsInitial { get; set; }

		public InputsInputNum InputsInputNum { get; set; }

		public InputsPoll InputsPoll { get; set; }

		public Autopoll Autopoll { get; set; }
	}
}