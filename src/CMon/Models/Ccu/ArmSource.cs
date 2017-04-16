namespace CMon.Models.Ccu
{
	/// <summary>
	/// Общие параметры источника изменения режима охраны
	/// </summary>
	public class ArmSource
	{
		public ArmSourceType Type { get; set; }

		// Дополнительные параметры для источника изменения режима охраны TouchMemory

		/// <summary>
		/// Номер ключа. Отсутствует  обязателен KeyName.
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Имя ключа. Отсутствует  обязателен Key.
		/// </summary>
		public string KeyName { get; set; }

		// Дополнительные параметры для источников изменения режима охраны DTMF, SMS, CSD, Call

		public string Phone { get; set; }
	}
}