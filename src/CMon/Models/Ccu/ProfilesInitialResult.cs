namespace CMon.Models.Ccu
{
	public class ProfilesInitialResult : CommandResult
	{
		public TopLine TopLine { get; set; }

		public DeviceInitial DeviceInitial { get; set; }
		
		public string[] InputsSchema { get; set; }
		
		public string[] OutputsSchema { get; set; }
		
		public string[] ProfilesSchema { get; set; }

		public ProfilesInitial ProfilesInitial { get; set; }

		public ProfilesProfNum ProfilesProfNum { get; set; }
	}
}