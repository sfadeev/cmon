using System;
using System.Collections.Generic;
using CMon.Models;
using CMon.Models.Ccu;

namespace CMon.Services
{
	public class DeviceEventDisplayResolver
	{
		private static readonly Dictionary<EventType, DisplayOptions> EventTypes = new Dictionary<EventType, DisplayOptions>
		{
			{ EventType.InputPassive, new DisplayOptions { Title = "Вход пассивен", Icon = "toggle-off" } },
			{ EventType.InputActive, new DisplayOptions { Title = "Вход активен", Icon = "toggle-on" } },
			{ EventType.PowerRecovery, new DisplayOptions { Title = "Восстановление внешнего питания", Icon = "plug" } },
			{ EventType.PowerFault, new DisplayOptions { Title = "Отключение внешнего питания", Icon = "power-off" } },
			{ EventType.BatteryLow1, new DisplayOptions { Title = "Разряд батареи до 1 уровня", Icon = "battery-half" } },
			{ EventType.BatteryLow2, new DisplayOptions { Title = "Разряд батареи до 2 уровня", Icon = "battery-quarter" } },
			{ EventType.BalanceLow, new DisplayOptions { Title = "Баланс снизился до минимального значения", Icon = "money-bill-alt" } },
			{ EventType.TempLow, new DisplayOptions { Title = "Температура платы упала до нижней границы", Icon = "thermometer-empty" } },
			{ EventType.TempNormal, new DisplayOptions { Title = "Температура платы вернулась в допустимый диапазон", Icon = "thermometer-half" } },
			{ EventType.TempHigh, new DisplayOptions { Title = "Температура платы поднялась до верхней границы", Icon = "thermometer-full" } },
			{ EventType.CaseOpen, new DisplayOptions { Title = "Вскрытие корпуса контроллера", Icon = "unlock-alt" } },
			{ EventType.Test, new DisplayOptions { Title = "Тестовое сообщение", Icon = "comment" } },
			{ EventType.Info, new DisplayOptions { Title = "Информационное сообщение", Icon = "info" } },
			{ EventType.Arm, new DisplayOptions { Title = "Переведен в режим ОХРАНА", Icon = "lock" } },
			{ EventType.Disarm, new DisplayOptions { Title = "Переведен в режим НАБЛЮДЕНИЕ", Icon = "lock-open" } },
			{ EventType.Protect, new DisplayOptions { Title = "Переведен в режим ЗАЩИТА", Icon = "unlock" } },
			{ EventType.ProfileApplied, new DisplayOptions { Title = "Применен профиль", Icon = "check-circle" } },
			{ EventType.DeviceOn, new DisplayOptions { Title = "Контроллер включен", Icon = "power-off" } },
			{ EventType.DeviceRestart, new DisplayOptions { Title = "Контроллер перезапущен", Icon = "sync" } },
			{ EventType.FirmwareUpgrade, new DisplayOptions { Title = "Прошивка обновлена", Icon = "wrench"} },
			{ EventType.ExtRuntimeError, new DisplayOptions { Title = "Ошибка выполнения программы EXT", Icon = "exclamation-triangle" } }
		};

		public void Resolve(DeviceEvent @event)
		{
			if (@event.EventType != null && Enum.TryParse<EventType>(@event.EventType, true, out var eventType))
			{
				if (EventTypes.TryGetValue(eventType, out var options))
				{
					@event.DisplayTitle = options.Title;
					@event.DisplayIcon = options.Icon;

					if (options.BuildParams != null)
					{
						@event.DisplayParams = options.BuildParams(@event);
					}
				}
			}
		}

		private class DisplayOptions
		{
			public string Title { get; set; }

			public string Icon { get; set; }

			public Func<DeviceEvent, string[]> BuildParams { get; set; }
		}
	}
}