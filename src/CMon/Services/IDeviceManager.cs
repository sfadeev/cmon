using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Models;
using CMon.Models.Ccu;
using CMon.Requests;
using LinqToDB;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CMon.Services
{
	public interface IDeviceManager
	{
		void Configure(long deviceId);

		string GetStatus();

		Task Refresh(RefreshDevice command);

		Task PollAsync(long deviceId);
	}

	public class CcuDeviceManager : IDeviceManager
	{
		public const short BoardTemp = 0xFF;

		private long _deviceId;
		private string _status = string.Empty;

		private readonly IMediator _mediator;
		private readonly IDbConnectionFactory _connectionFactory;
		private readonly IDeviceRepository _repository;
		private readonly ILogger<CcuDeviceManager> _logger;
		private readonly ICcuGateway _gateway;
		private readonly Sha1Hasher _hasher;

		public CcuDeviceManager(ILogger<CcuDeviceManager> logger,
			IMediator mediator, ICcuGateway gateway, Sha1Hasher hasher,
			IDbConnectionFactory connectionFactory, IDeviceRepository repository)
		{
			_logger = logger;
			_mediator = mediator;
			_gateway = gateway;
			_hasher = hasher;
			_connectionFactory = connectionFactory;
			_repository = repository;
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

		private async Task<DeviceConfig> GetConfig(Auth auth)
		{
			var result = new DeviceConfig();

			try
			{
				_status = "GetIndexInitialResult";
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

					_status = "GetInputsInitial";
					var inputsInitialResult = await _gateway.GetInputsInitial(auth);

					var inputs = new List<DeviceInput>();

					if (inputsInitialResult.Status.Code == StatusCode.Ok)
					{
						for (var inputNum = 0; inputNum < inputsInitialResult.InputsInitial.InCount; inputNum++)
						{
							_status = "GetInputsInputNum " + inputNum;
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

					_status = string.Empty;
				}
				else
				{
					_status = indexInitialResult.Status.Description;
				}
			}
			catch (Exception ex)
			{
				_status = ex.Message;
			}

			return result;
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
						if (input.Type == InputType.Rtd02 || input.Type == InputType.Rtd03)
						{
							t = GetInputTemperature(stateAndEvents.Inputs[input.InputNo - 1].Voltage);

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
			}
		}

		private static decimal GetInputTemperature(long discrete)
		{
			var voltage = discrete * 10M / 4095;

			var temp = (voltage / 5M - 0.5M) / 0.01M;

			return temp;
		}
	}
}