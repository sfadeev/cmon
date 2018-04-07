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
			{ EventType.BalanceLow, new DisplayOptions { Title = "Баланс снизился до минимального значения", Icon = "money" } },
			{ EventType.TempLow, new DisplayOptions { Title = "Температура платы упала до нижней границы", Icon = "thermometer-empty" } },
			{ EventType.TempNormal, new DisplayOptions { Title = "Температура платы вернулась в допустимый диапазон", Icon = "thermometer-half" } },
			{ EventType.TempHigh, new DisplayOptions { Title = "Температура платы поднялась до верхней границы", Icon = "thermometer-full" } },
			{ EventType.CaseOpen, new DisplayOptions { Title = "Вскрытие корпуса контроллера", Icon = "folder-open-o" } },
			{ EventType.Test, new DisplayOptions { Title = "Тестовое сообщение", Icon = "comment-o" } },
			{ EventType.Info, new DisplayOptions { Title = "Информационное сообщение", Icon = "info" } },
			{ EventType.Arm, new DisplayOptions { Title = "Переведен в режим ОХРАНА", Icon = "lock" } },
			{ EventType.Disarm, new DisplayOptions { Title = "Переведен в режим НАБЛЮДЕНИЕ", Icon = "eye" } },
			{ EventType.Protect, new DisplayOptions { Title = "Переведен в режим ЗАЩИТА", Icon = "unlock" } },
			{ EventType.ProfileApplied, new DisplayOptions { Title = "Применен профиль", Icon = "check-circle" } },
			{ EventType.DeviceOn, new DisplayOptions { Title = "Контроллер включен", Icon = "play" } },
			{ EventType.DeviceRestart, new DisplayOptions { Title = "Контроллер перезапущен", Icon = "repeat" } },
			{ EventType.FirmwareUpgrade, new DisplayOptions { Title = "Прошивка обновлена", Icon = "wrench"} },
			{ EventType.ExtRuntimeError, new DisplayOptions { Title = "Ошибка выполнения программы EXT", Icon = "exclamation-triangle" } }
		};

		private static readonly Dictionary<ArmSourceType, string> SourceTypes = new Dictionary<ArmSourceType, string>
		{
			{ ArmSourceType.Button, "Кнопка" },
			{ ArmSourceType.Input, "Вход" },
			{ ArmSourceType.Scheduler, "Планировщик задач" },
			{ ArmSourceType.Modbus, "GuardTracker по сети Modbus" },
			{ ArmSourceType.TouchMemory, "Ключ TouchMemory" },
			{ ArmSourceType.DTMF, "Голосовое меню" },
			{ ArmSourceType.SMS, "SMS команда" },
			{ ArmSourceType.CSD, "CSD соединение" },
			{ ArmSourceType.Call, "Вызов без соединения" },
			{ ArmSourceType.GTNet, "GuardTracker по сети" },
			{ ArmSourceType.uGuardNet, "uGuard по сети" },
			{ ArmSourceType.Shell, "CCU shell" }
		};

		private static readonly Dictionary<int, string> ErrorCodes = new Dictionary<int, string>
		{
			{ 1, "Неверный код инструкции." },
			{ 2, "Неверный код встроенной подпрограммы." },
			{ 3, "Деление на ноль." },
			{ 4, "Переполнение стека сверху." },
			{ 5, "Переполнение стека снизу." },
			{ 6, "Переполнение кода сверху." },
			{ 7, "Переполнение кода снизу." },
			{ 8, "Переполнение данных сверху." },
			{ 9, "Переполнение данных снизу." },
			{ 10, "Ошибка чтения флеш-памяти." },
			{ 11, "Превышено максимально допустимое время обработки события." },
			{ 1025, "Неверный параметр встроенной подпрограммы $get_input_state." },
			{ 1026, "Неверный параметр встроенной подпрограммы $get_input_value." },
			{ 1027, "Неверный параметр встроенной подпрограммы $get_sensor_value." },
			{ 1028, "Неверный параметр встроенной подпрограммы $get_output_state." },
			{ 1029, "Недопустимый вызов встроенной подпрограммы $get_arm_mode для данной модификации контроллера." },
			{ 1030, "Неверный параметр встроенной подпрограммы $get_part_arm_mode или недопустимый вызов для данной модификации контроллера." },
			{ 1041, "Неверный параметр встроенной подпрограммы $get_year." },
			{ 1042, "Неверный параметр встроенной подпрограммы $get_month." },
			{ 1043, "Неверный параметр встроенной подпрограммы $get_day." },
			{ 1044, "Неверный параметр встроенной подпрограммы $get_day_of_week." },
			{ 1045, "Неверный параметр встроенной подпрограммы $get_hour." },
			{ 1046, "Неверный параметр встроенной подпрограммы $get_minute." },
			{ 1047, "Неверный параметр встроенной подпрограммы $get_second." },
			{ 1048, "Неверный параметр встроенной подпрограммы $set_output_state." },
			{ 1049, "Неверный параметр встроенной подпрограммы $set_output_pulse." },
			{ 1050, "Неверный параметр встроенной подпрограммы $set_arm_mode или недопустимый вызов для данной модификации контроллера." },
			{ 1051, "Неверный параметр встроенной подпрограммы $set_part_arm_mode или недопустимый вызов для данной модификации контроллера." },
			{ 1052, "Неверный параметр встроенной подпрограммы $apply_profile." },
			{ 1053, "Неверный параметр встроенной подпрограммы $set_event_mask." },
			{ 1054, "Неверный параметр встроенной подпрограммы $reset_event_mask." },
			{ 1055, "Неверный параметр встроенной подпрограммы $set_timer." },
			{ 1056, "Неверный параметр встроенной подпрограммы $reset_timer." },
			{ 1057, "Неверный параметр встроенной подпрограммы $set_alarm." },
			{ 1058, "Неверный параметр встроенной подпрограммы $reset_alarm." },
			{ 1059, "Неверный параметр встроенной подпрограммы $raise_event." },
			{ 1060, "Неверный параметр встроенной подпрограммы $get_input_low_limit." },
			{ 1061, "Неверный параметр встроенной подпрограммы $get_input_high_limit." },
			{ 1062, "Неверный параметр встроенной подпрограммы $get_sensor_low_limit." },
			{ 1063, "Неверный параметр встроенной подпрограммы $get_sensor_high_limit." }
		};

		public void Resolve(DeviceEvent @event)
		{
			if (@event.EventType != null && Enum.TryParse<EventType>(@event.EventType, true, out var eventType))
			{
				if (EventTypes.TryGetValue(eventType, out var options))
				{
					@event.DisplayTitle = options.Title;
					@event.DisplayIcon = options.Icon;

					if (@event.Info != null)
					{
						@event.DisplayParams = BuildParams(eventType, @event.Info);
					}
				}
			}
		}

		private static IList<DeviceEventInfoParam> BuildParams(EventType eventType, DeviceEventInformation info)
		{
			var result = new List<DeviceEventInfoParam>();

			if (info?.Number != null)
			{
				if (eventType == EventType.InputActive || eventType == EventType.InputPassive)
				{
					// todo: resolve input name
					result.Add(new DeviceEventInfoParam { Name = "Номер входа", Value = info.Number.ToString() });
				}
				else if (eventType == EventType.ProfileApplied)
				{
					result.Add(new DeviceEventInfoParam { Name = "Номер профиля", Value = info.Number.ToString() });
				}
			}

			if (info?.Partitions?.Length > 0)
			{
				// todo: resolve partitions names
				result.Add(new DeviceEventInfoParam { Name = "Номера привязанных разделов", Value = string.Join(", ", info.Partitions) });
			}

			if (info?.Partition != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "Номер раздела", Value = info.Partition.ToString() });
			}

			if (info?.Source?.Type != null
				&& Enum.TryParse<ArmSourceType>(info.Source.Type, out var sourceType)
				&& SourceTypes.TryGetValue(sourceType, out var value))
			{
				result.Add(new DeviceEventInfoParam { Name = "Источник", Value = value });
			}

			if (info?.Source?.Key != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "Номер ключа", Value = info.Source.Key });
			}

			if (info?.Source?.KeyName != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "Имя ключа", Value = info.Source.KeyName });
			}

			if (info?.Source?.Phone != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "Номер телефона", Value = info.Source.Phone });
			}

			if (info?.UserName != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "Имя пользователя", Value = info.UserName });
			}

			if (info?.ErrorCode != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "Код ошибки", Value = info.ErrorCode.ToString() });

				if (ErrorCodes.TryGetValue(info.ErrorCode.Value, out var error))
				{
					result.Add(new DeviceEventInfoParam { Name = "Ошибка", Value = error });
				}
			}

			return result;
		}

		private class DisplayOptions
		{
			public string Title { get; set; }

			public string Icon { get; set; }
		}
	}
}