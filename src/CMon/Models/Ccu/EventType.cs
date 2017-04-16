namespace CMon.Models.Ccu
{
	public enum EventType
	{
		/// <summary>
		/// ���� ��������.
		/// </summary>
		InputPassive,

		/// <summary>
		/// ���� �������.
		/// </summary>
		InputActive,

		/// <summary>
		/// �������������� �������� �������.
		/// </summary>
		PowerRecovery,

		/// <summary>
		/// ���������� �������� �������.
		/// </summary>
		PowerFault,

		/// <summary>
		/// ������ ������� �� 1 ������.
		/// </summary>
		BatteryLow1,

		/// <summary>
		/// ������ ������� �� 2 ������.
		/// </summary>
		BatteryLow2,

		/// <summary>
		/// ������ �������� �� ������������ ��������.
		/// </summary>
		BalanceLow,

		/// <summary>
		/// ����������� ����� ����� �� ������ �������.
		/// </summary>
		TempLow,

		/// <summary>
		/// ����������� ����� ��������� � ���������� ��������.
		/// </summary>
		TempNormal,

		/// <summary>
		/// ����������� ����� ��������� �� ������� �������.
		/// </summary>
		TempHigh,

		/// <summary>
		/// �������� ������� �����������.
		/// </summary>
		CaseOpen,

		/// <summary>
		/// �������� ���������.
		/// </summary>
		Test,

		/// <summary>
		/// �������������� ���������.
		/// </summary>
		Info,

		/// <summary>
		/// ��������� � ����� ������.
		/// </summary>
		Arm,

		/// <summary>
		/// ��������� � ����� ����������.
		/// </summary>
		Disarm,

		/// <summary>
		/// ��������� � ����� ������.
		/// </summary>
		Protect,

		/// <summary>
		/// �������� �������.
		/// </summary>
		ProfileApplied,

		/// <summary>
		/// ���������� �������.
		/// </summary>
		DeviceOn,

		/// <summary>
		/// ���������� �����������.
		/// </summary>
		DeviceRestart,

		/// <summary>
		/// �������� ���������.
		/// </summary>
		FirmwareUpgrade,

		/// <summary>
		/// ������ ���������� ��������� EXT.
		/// </summary>
		ExtRuntimeError
	}
}