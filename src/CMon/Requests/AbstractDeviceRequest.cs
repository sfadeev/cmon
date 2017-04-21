namespace CMon.Requests
{
	public abstract class AbstractDeviceRequest
	{
		public string UserName { get; set; }

		public long DeviceId { get; set; }
	}
}