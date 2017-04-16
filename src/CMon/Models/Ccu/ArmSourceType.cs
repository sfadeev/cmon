namespace CMon.Models.Ccu
{
	/// <summary>
	/// ���� ��������� ��������� ������ ������
	/// </summary>
	public enum ArmSourceType
	{
		/// <summary>
		/// ������.
		/// </summary>
		Button,

		/// <summary>
		/// ����.
		/// </summary>
		Input,

		/// <summary>
		/// ����������� �����.
		/// </summary>
		Scheduler,

		/// <summary>
		/// GuardTracker �� ���� Modbus.
		/// </summary>
		Modbus,

		/// <summary>
		/// ���� TouchMemory.
		/// </summary>
		TouchMemory,

		/// <summary>
		/// ��������� ����.
		/// </summary>
		DTMF,

		/// <summary>
		/// SMS �������.
		/// </summary>
		SMS,

		/// <summary>
		/// CSD ����������.
		/// </summary>
		CSD,

		/// <summary>
		/// ����� ��� ����������.
		/// </summary>
		Call,

		/// <summary>
		/// GuardTracker �� ����.
		/// </summary>
		GTNet,

		/// <summary>
		/// uGuard �� ����.
		/// </summary>
		uGuardNet,

		/// <summary>
		/// CCU shell.
		/// </summary>
		Shell
	}
}