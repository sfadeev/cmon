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
			{ EventType.BalanceLow, new DisplayOptions { Title = "������ �������� �� ������������ ��������", Icon = "money-bill-alt" } },
			{ EventType.TempLow, new DisplayOptions { Title = "����������� ����� ����� �� ������ �������", Icon = "thermometer-empty" } },
			{ EventType.TempNormal, new DisplayOptions { Title = "����������� ����� ��������� � ���������� ��������", Icon = "thermometer-half" } },
			{ EventType.TempHigh, new DisplayOptions { Title = "����������� ����� ��������� �� ������� �������", Icon = "thermometer-full" } },
			{ EventType.CaseOpen, new DisplayOptions { Title = "�������� ������� �����������", Icon = "unlock-alt" } },
			{ EventType.Test, new DisplayOptions { Title = "�������� ���������", Icon = "comment" } },
			{ EventType.Info, new DisplayOptions { Title = "�������������� ���������", Icon = "info" } },
			{ EventType.Arm, new DisplayOptions { Title = "��������� � ����� ������", Icon = "lock" } },
			{ EventType.Disarm, new DisplayOptions { Title = "��������� � ����� ����������", Icon = "lock-open" } },
			{ EventType.Protect, new DisplayOptions { Title = "��������� � ����� ������", Icon = "unlock" } },
			{ EventType.ProfileApplied, new DisplayOptions { Title = "�������� �������", Icon = "check-circle" } },
			{ EventType.DeviceOn, new DisplayOptions { Title = "���������� �������", Icon = "power-off" } },
			{ EventType.DeviceRestart, new DisplayOptions { Title = "���������� �����������", Icon = "sync" } },
			{ EventType.FirmwareUpgrade, new DisplayOptions { Title = "�������� ���������", Icon = "wrench"} },
			{ EventType.ExtRuntimeError, new DisplayOptions { Title = "������ ���������� ��������� EXT", Icon = "exclamation-triangle" } }
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