namespace CMon.Models.Ccu
{
	public class InputsInitialResult : CommandResult
	{
		public CurrentUser CurrentUser { get; set; }

		public DeviceInitial DeviceInitial { get; set; }

		public InputsInitial InputsInitial { get; set; }

		public InputsInputNum InputsInputNum { get; set; }

		public InputsPoll InputsPoll { get; set; }

		public Autopoll Autopoll { get; set; }
	}
}