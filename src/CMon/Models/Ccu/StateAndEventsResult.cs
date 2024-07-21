﻿namespace CMon.Models.Ccu
{
	public class StateAndEventsResult : CommandResult
	{
		public Input[] Inputs { get; set; }

		public byte[] Outputs { get; set; }

		public string[] Partitions { get; set; }

		public Battery Battery { get; set; }

		public byte Case { get; set; }

		public float Power { get; set; }

		public long Temp { get; set; }

		public float Balance { get; set; }

		public Event[] Events { get; set; }
	}
}