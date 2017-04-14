namespace CMon.Models.Ccu
{
	public class Temperature
	{
		public int L { get; set; }

		public int LowEdge { get; set; }

		public int HighEdge { get; set; }

		public bool NotificationsOn { get; set; }

		public int LowEdgeAct { get; set; }

		public int HighEdgeAct { get; set; }

		public int LowEdgeReactOut { get; set; }

		public int LowEdgeReact { get; set; }

		public int HighEdgeReactOut { get; set; }

		public int HighEdgeReact { get; set; }
	}
}