namespace CMon.Models.Ccu
{
	public class Event
	{
		public long Id { get; set; }

		public string Type { get; set; }

		// Дополнительные параметры для событий InputActive и InputPassive
		// Дополнительные параметры для события ProfileApplied

		public int? Number { get; set; }

		public int[] Partitions { get; set; }

		// Дополнительные параметры для событий Arm, Disarm, Protect

		public int? Partition { get; set; }
		
		public ArmSource Source { get; set; }

		// Дополнительные параметры для источников изменения режима охраны uGuardNet и Shell

		public string UserName { get; set; }

		// Дополнительный параметр для события ExtRuntimeError

		public int? ErrorCode { get; set; }
	}
}