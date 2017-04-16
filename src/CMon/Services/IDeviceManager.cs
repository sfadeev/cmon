using System;
using System.Collections.Generic;
using System.Linq;
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
	}

	public class CcuDeviceManager : IDeviceManager
	{
		private long _deviceId;
		private string _status = string.Empty;

		private readonly IMediator _mediator;
		private readonly IDbConnectionFactory _connectionFactory;
		private readonly ILogger<CcuDeviceManager> _logger;
		private readonly ICcuGateway _gateway;
		private readonly Sha1Hasher _hasher;

		public CcuDeviceManager(ILogger<CcuDeviceManager> logger,
			IMediator mediator, ICcuGateway gateway, Sha1Hasher hasher, IDbConnectionFactory connectionFactory)
		{
			_logger = logger;
			_mediator = mediator;
			_gateway = gateway;
			_hasher = hasher;
			_connectionFactory = connectionFactory;
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
	}
}