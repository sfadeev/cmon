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
			{ EventType.InputPassive, new DisplayOptions { Title = "���� ��������", Icon = "toggle-off" } },
			{ EventType.InputActive, new DisplayOptions { Title = "���� �������", Icon = "toggle-on" } },
			{ EventType.PowerRecovery, new DisplayOptions { Title = "�������������� �������� �������", Icon = "plug" } },
			{ EventType.PowerFault, new DisplayOptions { Title = "���������� �������� �������", Icon = "power-off" } },
			{ EventType.BatteryLow1, new DisplayOptions { Title = "������ ������� �� 1 ������", Icon = "battery-half" } },
			{ EventType.BatteryLow2, new DisplayOptions { Title = "������ ������� �� 2 ������", Icon = "battery-quarter" } },
			{ EventType.BalanceLow, new DisplayOptions { Title = "������ �������� �� ������������ ��������", Icon = "money" } },
			{ EventType.TempLow, new DisplayOptions { Title = "����������� ����� ����� �� ������ �������", Icon = "thermometer-empty" } },
			{ EventType.TempNormal, new DisplayOptions { Title = "����������� ����� ��������� � ���������� ��������", Icon = "thermometer-half" } },
			{ EventType.TempHigh, new DisplayOptions { Title = "����������� ����� ��������� �� ������� �������", Icon = "thermometer-full" } },
			{ EventType.CaseOpen, new DisplayOptions { Title = "�������� ������� �����������", Icon = "folder-open-o" } },
			{ EventType.Test, new DisplayOptions { Title = "�������� ���������", Icon = "comment-o" } },
			{ EventType.Info, new DisplayOptions { Title = "�������������� ���������", Icon = "info" } },
			{ EventType.Arm, new DisplayOptions { Title = "��������� � ����� ������", Icon = "lock" } },
			{ EventType.Disarm, new DisplayOptions { Title = "��������� � ����� ����������", Icon = "eye" } },
			{ EventType.Protect, new DisplayOptions { Title = "��������� � ����� ������", Icon = "unlock" } },
			{ EventType.ProfileApplied, new DisplayOptions { Title = "�������� �������", Icon = "check-circle" } },
			{ EventType.DeviceOn, new DisplayOptions { Title = "���������� �������", Icon = "play" } },
			{ EventType.DeviceRestart, new DisplayOptions { Title = "���������� �����������", Icon = "repeat" } },
			{ EventType.FirmwareUpgrade, new DisplayOptions { Title = "�������� ���������", Icon = "wrench"} },
			{ EventType.ExtRuntimeError, new DisplayOptions { Title = "������ ���������� ��������� EXT", Icon = "exclamation-triangle" } }
		};

		private static readonly Dictionary<ArmSourceType, string> SourceTypes = new Dictionary<ArmSourceType, string>
		{
			{ ArmSourceType.Button, "������" },
			{ ArmSourceType.Input, "����" },
			{ ArmSourceType.Scheduler, "����������� �����" },
			{ ArmSourceType.Modbus, "GuardTracker �� ���� Modbus" },
			{ ArmSourceType.TouchMemory, "���� TouchMemory" },
			{ ArmSourceType.DTMF, "��������� ����" },
			{ ArmSourceType.SMS, "SMS �������" },
			{ ArmSourceType.CSD, "CSD ����������" },
			{ ArmSourceType.Call, "����� ��� ����������" },
			{ ArmSourceType.GTNet, "GuardTracker �� ����" },
			{ ArmSourceType.uGuardNet, "uGuard �� ����" },
			{ ArmSourceType.Shell, "CCU shell" }
		};

		private static readonly Dictionary<int, string> ErrorCodes = new Dictionary<int, string>
		{
			{ 1, "�������� ��� ����������." },
			{ 2, "�������� ��� ���������� ������������." },
			{ 3, "������� �� ����." },
			{ 4, "������������ ����� ������." },
			{ 5, "������������ ����� �����." },
			{ 6, "������������ ���� ������." },
			{ 7, "������������ ���� �����." },
			{ 8, "������������ ������ ������." },
			{ 9, "������������ ������ �����." },
			{ 10, "������ ������ ����-������." },
			{ 11, "��������� ����������� ���������� ����� ��������� �������." },
			{ 1025, "�������� �������� ���������� ������������ $get_input_state." },
			{ 1026, "�������� �������� ���������� ������������ $get_input_value." },
			{ 1027, "�������� �������� ���������� ������������ $get_sensor_value." },
			{ 1028, "�������� �������� ���������� ������������ $get_output_state." },
			{ 1029, "������������ ����� ���������� ������������ $get_arm_mode ��� ������ ����������� �����������." },
			{ 1030, "�������� �������� ���������� ������������ $get_part_arm_mode ��� ������������ ����� ��� ������ ����������� �����������." },
			{ 1041, "�������� �������� ���������� ������������ $get_year." },
			{ 1042, "�������� �������� ���������� ������������ $get_month." },
			{ 1043, "�������� �������� ���������� ������������ $get_day." },
			{ 1044, "�������� �������� ���������� ������������ $get_day_of_week." },
			{ 1045, "�������� �������� ���������� ������������ $get_hour." },
			{ 1046, "�������� �������� ���������� ������������ $get_minute." },
			{ 1047, "�������� �������� ���������� ������������ $get_second." },
			{ 1048, "�������� �������� ���������� ������������ $set_output_state." },
			{ 1049, "�������� �������� ���������� ������������ $set_output_pulse." },
			{ 1050, "�������� �������� ���������� ������������ $set_arm_mode ��� ������������ ����� ��� ������ ����������� �����������." },
			{ 1051, "�������� �������� ���������� ������������ $set_part_arm_mode ��� ������������ ����� ��� ������ ����������� �����������." },
			{ 1052, "�������� �������� ���������� ������������ $apply_profile." },
			{ 1053, "�������� �������� ���������� ������������ $set_event_mask." },
			{ 1054, "�������� �������� ���������� ������������ $reset_event_mask." },
			{ 1055, "�������� �������� ���������� ������������ $set_timer." },
			{ 1056, "�������� �������� ���������� ������������ $reset_timer." },
			{ 1057, "�������� �������� ���������� ������������ $set_alarm." },
			{ 1058, "�������� �������� ���������� ������������ $reset_alarm." },
			{ 1059, "�������� �������� ���������� ������������ $raise_event." },
			{ 1060, "�������� �������� ���������� ������������ $get_input_low_limit." },
			{ 1061, "�������� �������� ���������� ������������ $get_input_high_limit." },
			{ 1062, "�������� �������� ���������� ������������ $get_sensor_low_limit." },
			{ 1063, "�������� �������� ���������� ������������ $get_sensor_high_limit." }
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
					result.Add(new DeviceEventInfoParam { Name = "����� �����", Value = info.Number.ToString() });
				}
				else if (eventType == EventType.ProfileApplied)
				{
					result.Add(new DeviceEventInfoParam { Name = "����� �������", Value = info.Number.ToString() });
				}
			}

			if (info?.Partitions?.Length > 0)
			{
				// todo: resolve partitions names
				result.Add(new DeviceEventInfoParam { Name = "������ ����������� ��������", Value = string.Join(", ", info.Partitions) });
			}

			if (info?.Partition != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "����� �������", Value = info.Partition.ToString() });
			}

			if (info?.Source?.Type != null
				&& Enum.TryParse<ArmSourceType>(info.Source.Type, out var sourceType)
				&& SourceTypes.TryGetValue(sourceType, out var value))
			{
				result.Add(new DeviceEventInfoParam { Name = "��������", Value = value });
			}

			if (info?.Source?.Key != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "����� �����", Value = info.Source.Key });
			}

			if (info?.Source?.KeyName != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "��� �����", Value = info.Source.KeyName });
			}

			if (info?.Source?.Phone != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "����� ��������", Value = info.Source.Phone });
			}

			if (info?.UserName != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "��� ������������", Value = info.UserName });
			}

			if (info?.ErrorCode != null)
			{
				result.Add(new DeviceEventInfoParam { Name = "��� ������", Value = info.ErrorCode.ToString() });

				if (ErrorCodes.TryGetValue(info.ErrorCode.Value, out var error))
				{
					result.Add(new DeviceEventInfoParam { Name = "������", Value = error });
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