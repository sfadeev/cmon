namespace CMon.Models.Ccu
{
	public class StateAndEvents : CommandResult
	{
		public Input[] Inputs { get; set; }

		public byte[] Outputs { get; set; }

		public string[] Partitions { get; set; }

		public Battery Battery { get; set; }

		public byte Case { get; set; }

		public string Power { get; set; }

		public long Temp { get; set; }

		public float Balance { get; set; }
	}
}