using CMon.Models.Ccu;

namespace CMon.Models
{
	public class DeviceEventInformation
	{
		// �������������� ��������� ��� ������� InputActive � InputPassive
		// �������������� ��������� ��� ������� ProfileApplied

		public int? Number { get; set; }

		public int[] Partitions { get; set; }

		// �������������� ��������� ��� ������� Arm, Disarm, Protect

		public int? Partition { get; set; }

		public ArmSource Source { get; set; }

		// �������������� ��������� ��� ���������� ��������� ������ ������ uGuardNet � Shell

		public string UserName { get; set; }

		// �������������� �������� ��� ������� ExtRuntimeError

		public int? ErrorCode { get; set; }
	}
}