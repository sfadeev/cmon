namespace CMon.Models.Ccu
{
	public class InputsInputNum
	{
		public int L { get; set; }

		public short InputNum { get; set; }

		public bool Enable { get; set; }

		public InputType InputType { get; set; }

		public string InputName { get; set; }

		public string InputActiveName { get; set; }

		public string InputPassiveName { get; set; }

		public AlarmZone AlarmZone { get; set; }

		public bool AllDayControl { get; set; }

		public int DiscardShortImp { get; set; }

		public int MinActCount { get; set; }

		public int MinActCountTime { get; set; }

		public int AlarmDelay { get; set; }

		public int InputControlRecoveryDelay { get; set; }

		public int AlarmsLimitInArmSession { get; set; }

		public bool NoArmWhenActive { get; set; }

		public int VoiceCommOnAlarmConn { get; set; }

		public int OutsCtrlSrc { get; set; }

		public int[] InActiveReact { get; set; }

		public int[] InPassiveReact { get; set; }
	}
}