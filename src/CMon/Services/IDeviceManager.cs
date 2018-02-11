using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Hubs;
using CMon.Models;
using CMon.Models.Ccu;
using CMon.Requests;
using LinqToDB;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CMon.Services
{
	public interface IDeviceManager
	{
		void Configure(long deviceId);

		string GetStatus();

		Task Refresh(RefreshDevice command);

		void StartPolling();

		void StopPolling();
	}

	public class DeviceOptions
	{
		public bool PollingEnabled { get; set; }

		public int PollingIntervalSeconds { get; set; }
	}

	public class CcuDeviceManager : IDeviceManager
	{
		public const short BoardTemp = 0xFF;

		private long _deviceId;
		private string _status = string.Empty;

		private readonly IMediator _mediator;
		private readonly IDbConnectionFactory _connectionFactory;
		private readonly IDeviceRepository _repository;
		private readonly DashboardNotifier _dashboardNotifier;
		private readonly ILogger<CcuDeviceManager> _logger;
		private readonly IOptions<DeviceOptions> _options;
		private readonly ICcuGateway _gateway;
		private readonly Sha1Hasher _hasher;

		public CcuDeviceManager(ILogger<CcuDeviceManager> logger, IOptions<DeviceOptions> options,
			IMediator mediator, ICcuGateway gateway, Sha1Hasher hasher,
			IDbConnectionFactory connectionFactory, IDeviceRepository repository,
			DashboardNotifier dashboardNotifier)
		{
			_logger = logger;
			_options = options;
			_mediator = mediator;
			_gateway = gateway;
			_hasher = hasher;
			_connectionFactory = connectionFactory;
			_repository = repository;
			_dashboardNotifier = dashboardNotifier;
		}

		public void Configure(long deviceId)
		{
			_deviceId = deviceId;
		}

		public string GetStatus()
		{
			return _status;
		}

		public async Task Refresh(RefreshDevice command)
		{
			var device = await _mediator.Send(
				new GetDevice { DeviceId = command.DeviceId, UserName = command.UserName, WithAuth = true });

			if (device != null)
			{
				var config = await GetConfig(device.Auth);

				if (config != null)
				{
					var hash = _hasher.Compute(config);

					if (device.Hash == null || device.Hash.SequenceEqual(hash) == false)
					{
						_logger.LogDebug(
							device.Hash == null
								? "Initial device {0} configuration insert."
								: "Updating device {0} configuration.", device.Id);

						var now = DateTime.UtcNow;

						using (var db = _connectionFactory.GetConection())
						{
							using (var transaction = db.BeginTransaction())
							{
								// update device
								db.GetTable<DbDevice>()
									.Where(x => x.Id == device.Id)
									.Set(x => x.Config, JsonConvert.SerializeObject(config))
									.Set(x => x.Hash, hash)
									.Set(x => x.ModifiedAt, now)
									.Set(x => x.ModifiedBy, command.UserName)
									.Update();

								// todo: add operations log

								transaction.Commit();
							}
						}
					}
					else
					{
						_logger.LogDebug("Device {0} configuration is not changed.", device.Id);
					}
				}
			}
		}

		private readonly object _timerLock = new object ();

		private Timer _timer;

		public void StartPolling()
		{
			if (_options.Value.PollingEnabled)
			{
				_timer = new Timer(state =>
				{
					var lockTaken = false;

					try
					{
						lockTaken = Monitor.TryEnter(_timerLock);

						if (lockTaken)
						{
							try
							{
								PollAsync(_deviceId).Wait();
							}
							catch (Exception ex)
							{
								_logger.LogError(0, ex, "Error sending request for device {deviceId}", _deviceId);
							}
						}
						else
						{
							_logger.LogDebug(0, "Skip polling device {deviceId} - polling already running.", _deviceId);
						}
					}
					finally
					{
						if (lockTaken) Monitor.Exit(_timerLock);
					}
				}, _deviceId, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(_options.Value.PollingIntervalSeconds));
			}
		}

		public void StopPolling()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}

		private async Task<DeviceConfig> GetConfig(Auth auth)
		{
			var result = new DeviceConfig();

			try
			{
				Log("GetIndexInitialResult");
				var indexInitialResult = await _gateway.GetIndexInitial(auth);

				if (indexInitialResult.Status.Code == StatusCode.Ok)
				{
					var deviceInfo = indexInitialResult.IndexInitial.DeviceInfo;

					result.Info = new DeviceInformation
					{
						DeviceType = deviceInfo.DeviceType,
						DeviceMod = deviceInfo.DeviceMod,
						HwVer = deviceInfo.HwVer,
						FwVer = deviceInfo.FwVer,
						BootVer = deviceInfo.BootVer,
						FwBuildDate = deviceInfo.FwBuildDate,
						CountryCode = deviceInfo.CountryCode,
						Serial = deviceInfo.Serial,
						Imei = deviceInfo.Imei,
						InputsCount = deviceInfo.InputsCount,
						PartitionsCount = deviceInfo.PartitionsCount,
						ExtBoard = deviceInfo.ExtBoard,
						uGuardVerCode = deviceInfo.uGuardVerCode
					};

					Log("GetInputsInitial");
					var inputsInitialResult = await _gateway.GetInputsInitial(auth);

					var inputs = new List<DeviceInput>();

					if (inputsInitialResult.Status.Code == StatusCode.Ok)
					{
						for (var inputNum = 0; inputNum < inputsInitialResult.InputsInitial.InCount; inputNum++)
						{
							Log("GetInputsInputNum " + inputNum);
							var inputNumResult = await _gateway.GetInputsInputNum(auth, inputNum);

							if (inputNumResult.Status.Code == StatusCode.Ok)
							{
								var ccuInput = inputNumResult.InputsInputNum;

								inputs.Add(new DeviceInput
								{
									InputNo = (short) (ccuInput.InputNum + 1),
									Type = (InputType) ccuInput.InputType,
									Name = ccuInput.InputName,
									ActiveName = ccuInput.InputActiveName,
									PassiveName = ccuInput.InputPassiveName,
									AlarmZoneRangeType = (RangeType) ccuInput.AlarmZone.RangeType,
									AlarmZoneMinValue = ccuInput.AlarmZone.UserMinVal,
									AlarmZoneMaxValue = ccuInput.AlarmZone.UserMaxVal
								});
							}
						}
					}

					result.Inputs = inputs.ToArray();

					Log(string.Empty);

					return result;
				}
				else
				{
					Log(indexInitialResult.Status.Description);
				}
			}
			catch (Exception ex)
			{
				Log(ex.GetType().Name + " - " + ex.Message);

			}

			return null;
		}

		private void Log(string message)
		{
			_dashboardNotifier.Notify(hub => hub.Log(_deviceId, message));
		}

		private void SetStatus(string status)
		{
			_status = status;

			_dashboardNotifier.Notify(hub => hub.OnStatusUpdated(_deviceId, status));
		}
		
		public async Task PollAsync(long deviceId)
		{
			var device = await _mediator.Send(
				new GetDevice { DeviceId = deviceId, WithAuth = true });

			var stateAndEvents = await _gateway.GetStateAndEvents(device.Auth);

			// todo: check 429 (Too Many Requests)
			if (stateAndEvents != null && stateAndEvents.HttpStatusCode == HttpStatusCode.OK)
			{
				var t = (decimal)stateAndEvents.Temp;

				_repository.SaveInputValue(device.Id, BoardTemp, t);

				var message = $"[{device.Id}] - {BoardTemp}:{t:N4}";

				if (device.Config?.Inputs != null)
				{
					foreach (var input in device.Config.Inputs)
					{
						if (input.Type == InputType.Rtd02 || input.Type == InputType.Rtd03 || input.Type == InputType.Rtd04)
						{
							t = GetInputTemperature(input, stateAndEvents.Inputs[input.InputNo - 1].Voltage);

							_repository.SaveInputValue(device.Id, input.InputNo, t);

							message += $" - {input.InputNo}:{t:N4}";
						}
					}
				}

				if (stateAndEvents.Events != null)
				{
					var events = new List<DeviceEvent>();

					foreach (var @event in stateAndEvents.Events)
					{
						events.Add(new DeviceEvent
						{
							EventType = @event.Type,
							ExternalId = @event.Id,
							Info = new DeviceEventInformation
							{
								Number = @event.Number,
								Partitions = @event.Partitions,
								Partition = @event.Partition,
								Source = @event.Source,
								UserName = @event.UserName,
								ErrorCode = @event.ErrorCode
							}
						});
					}

					var saved = _repository.SaveEvents(device.Id, events);

					var savedIds = saved
						.Select(x => x.ExternalId)
						.Where(x => x.HasValue)
						.Cast<long>()
						.ToArray();

					if (savedIds.Length > 0)
					{
						await _gateway.AckEvents(device.Auth, savedIds);
					}
				}

				_logger.LogInformation(message);

				SetStatus(message);
			}
		}

		private static decimal GetInputTemperature(DeviceInput input, long discrete)
		{
			var voltage = discrete * 10M / MAX_RANGE_VAL;

			decimal temp;

			if (input.Type == InputType.Rtd02)
			{
				temp = (voltage / 3M - 0.5M) / 0.01M;
			}
			else if (input.Type == InputType.Rtd03)
			{
				temp = (voltage / 5M - 0.5M) / 0.01M;
			}
			else if (input.Type == InputType.Rtd04)
			{
				temp = -3.03641M * (decimal)Math.Pow((double)voltage, 3) 
						+ 25.5916M * (decimal)Math.Pow((double)voltage, 2) 
						- 87.9556M * voltage + 120.641M;

				// temp = -40.3289M * (decimal)Math.Log(0.28738 * (double)voltage);
			}
			else
			{
				throw new InvalidOperationException($"Input type {input.Type} is not supported.");
			}

			return temp;
		}

		private const decimal MAX_RANGE_VAL = 4095;
	}
}