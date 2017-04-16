using CMon.Models.Ccu;

namespace CMon.Models
{
	public class DeviceEventInformation
	{
		// ƒополнительные параметры дл€ событий InputActive и InputPassive
		// ƒополнительные параметры дл€ событи€ ProfileApplied

		public int? Number { get; set; }

		public int[] Partitions { get; set; }

		// ƒополнительные параметры дл€ событий Arm, Disarm, Protect

		public int? Partition { get; set; }

		public ArmSource Source { get; set; }

		// ƒополнительные параметры дл€ источников изменени€ режима охраны uGuardNet и Shell

		public string UserName { get; set; }

		// ƒополнительный параметр дл€ событи€ ExtRuntimeError

		public int? ErrorCode { get; set; }
	}
}