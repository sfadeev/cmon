namespace CMon.Models.Ccu
{
	public enum BatteryState
	{
		/// <summary>
		/// �����
		/// </summary>
		OK,

		/// <summary>
		/// �� ��������������
		/// </summary>
		NotUsed,

		/// <summary>
		/// ���������
		/// </summary>
		Disconnected,

		/// <summary>
		/// ������ �� 1 ������
		/// </summary>
		Low1,

		/// <summary>
		/// ������ �� 2 ������
		/// </summary>
		Low2
	}
}