namespace CMon.Models.Ccu
{
	/// <summary>
	/// Типы источника изменения режима охраны
	/// </summary>
	public enum ArmSourceType
	{
		/// <summary>
		/// Кнопка.
		/// </summary>
		Button,

		/// <summary>
		/// Вход.
		/// </summary>
		Input,

		/// <summary>
		/// Планировщик задач.
		/// </summary>
		Scheduler,

		/// <summary>
		/// GuardTracker по сети Modbus.
		/// </summary>
		Modbus,

		/// <summary>
		/// Ключ TouchMemory.
		/// </summary>
		TouchMemory,

		/// <summary>
		/// Голосовое меню.
		/// </summary>
		DTMF,

		/// <summary>
		/// SMS команда.
		/// </summary>
		SMS,

		/// <summary>
		/// CSD соединение.
		/// </summary>
		CSD,

		/// <summary>
		/// Вызов без соединения.
		/// </summary>
		Call,

		/// <summary>
		/// GuardTracker по сети.
		/// </summary>
		GTNet,

		/// <summary>
		/// uGuard по сети.
		/// </summary>
		uGuardNet,

		/// <summary>
		/// CCU shell.
		/// </summary>
		Shell
	}
}