namespace CMon.Models.Ccu
{
	public enum EventType
	{
		/// <summary>
		/// Вход пассивен.
		/// </summary>
		InputPassive,

		/// <summary>
		/// Вход активен.
		/// </summary>
		InputActive,

		/// <summary>
		/// Восстановление внешнего питания.
		/// </summary>
		PowerRecovery,

		/// <summary>
		/// Отключение внешнего питания.
		/// </summary>
		PowerFault,

		/// <summary>
		/// Разряд батареи до 1 уровня.
		/// </summary>
		BatteryLow1,

		/// <summary>
		/// Разряд батареи до 2 уровня.
		/// </summary>
		BatteryLow2,

		/// <summary>
		/// Баланс снизился до минимального значения.
		/// </summary>
		BalanceLow,

		/// <summary>
		/// Температура платы упала до нижней границы.
		/// </summary>
		TempLow,

		/// <summary>
		/// Температура платы вернулась в допустимый диапазон.
		/// </summary>
		TempNormal,

		/// <summary>
		/// Температура платы поднялась до верхней границы.
		/// </summary>
		TempHigh,

		/// <summary>
		/// Вскрытие корпуса контроллера.
		/// </summary>
		CaseOpen,

		/// <summary>
		/// Тестовое сообщение.
		/// </summary>
		Test,

		/// <summary>
		/// Информационное сообщение.
		/// </summary>
		Info,

		/// <summary>
		/// Переведен в режим ОХРАНА.
		/// </summary>
		Arm,

		/// <summary>
		/// Переведен в режим НАБЛЮДЕНИЕ.
		/// </summary>
		Disarm,

		/// <summary>
		/// Переведен в режим ЗАЩИТА.
		/// </summary>
		Protect,

		/// <summary>
		/// Применен профиль.
		/// </summary>
		ProfileApplied,

		/// <summary>
		/// Контроллер включен.
		/// </summary>
		DeviceOn,

		/// <summary>
		/// Контроллер перезапущен.
		/// </summary>
		DeviceRestart,

		/// <summary>
		/// Прошивка обновлена.
		/// </summary>
		FirmwareUpgrade,

		/// <summary>
		/// Ошибка выполнения программы EXT.
		/// </summary>
		ExtRuntimeError
	}
}