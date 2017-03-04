namespace CMon.Models.Ccu
{
	public class ProfilesInitialResult : CommandResult
	{
		public CurrentUser CurrentUser { get; set; }

		public DeviceInitial DeviceInitial { get; set; }

		public ProfilesInitial ProfilesInitial { get; set; }

		public ProfilesProfNum ProfilesProfNum { get; set; }
	}
}