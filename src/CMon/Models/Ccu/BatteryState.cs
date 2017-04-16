namespace CMon.Models.Ccu
{
	public enum BatteryState
	{
		/// <summary>
		/// норма
		/// </summary>
		OK,

		/// <summary>
		/// не использовалась
		/// </summary>
		NotUsed,

		/// <summary>
		/// отключена
		/// </summary>
		Disconnected,

		/// <summary>
		/// разряд до 1 уровня
		/// </summary>
		Low1,

		/// <summary>
		/// разряд до 2 уровня
		/// </summary>
		Low2
	}
}