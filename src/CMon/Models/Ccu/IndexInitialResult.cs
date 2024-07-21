namespace CMon.Models.Ccu
{
	public class IndexInitialResult : CommandResult
	{
		public TopLine TopLine { get; set; }

		public DeviceInitial DeviceInitial { get; set; }

		public Langs Langs { get; set; }

		public IndexInitial IndexInitial { get; set; }
	}
}