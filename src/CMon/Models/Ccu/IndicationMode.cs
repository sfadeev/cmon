namespace CMon.Models.Ccu
{
	public enum IndicationMode
	{
		/// <summary>
		/// ���������� ����� 1-8 �� ����������� In1-In8
		/// </summary>
		M0 = 0,

		/// <summary>
		/// ���������� ������� ������� GSM �� ����������� In1-In8
		/// </summary>
		Gsm = 1,

		/// <summary>
		/// ���������� ����� 9-16 �� ����������� In1-In8
		/// </summary>
		M2 = 2,

		/// <summary>
		/// ����������� ����� ����������� ���������
		/// </summary>
		Ext = 3
	}
}