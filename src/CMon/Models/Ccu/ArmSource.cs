namespace CMon.Models.Ccu
{
	/// <summary>
	/// ����� ��������� ��������� ��������� ������ ������
	/// </summary>
	public class ArmSource
	{
		public string Type { get; set; }

		// �������������� ��������� ��� ��������� ��������� ������ ������ TouchMemory

		/// <summary>
		/// ����� �����. �����������  ���������� KeyName.
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// ��� �����. �����������  ���������� Key.
		/// </summary>
		public string KeyName { get; set; }

		// �������������� ��������� ��� ���������� ��������� ������ ������ DTMF, SMS, CSD, Call

		public string Phone { get; set; }
	}
}