namespace CMon.Models.Ccu
{
	public class Status
	{
		public StatusCode Code { get; set; }

		public string Description { get; set; }
	}

	public enum StatusCode
	{
		Ok = 0,
		InsufficientRights = -9
	}
}