namespace CMon.Models.Ccu
{
	public enum IndicationMode
	{
		/// <summary>
		/// Показывать входы 1-8 на индикаторах In1-In8
		/// </summary>
		M0 = 0,

		/// <summary>
		/// Показывать уровень сигнала GSM на индикаторах In1-In8
		/// </summary>
		Gsm = 1,

		/// <summary>
		/// Показывать входы 9-16 на индикаторах In1-In8
		/// </summary>
		M2 = 2,

		/// <summary>
		/// Установлена плата расширенной индикации
		/// </summary>
		Ext = 3
	}
}