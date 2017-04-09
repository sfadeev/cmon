using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Models.Ccu;
using CMon.Requests;
using LinqToDB;
using MediatR;
using Microsoft.Extensions.Logging;

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
		private string _status;

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
				var inputs = await GetInputs(device.Auth);

				var hash = _hasher.ComputeHash(inputs);

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
							// remove current inputs
							db.GetTable<DbInput>()
								.Where(x => x.DeviceId == device.Id)
								.Delete();

							// insert new inputs
							// todo: read other device info (model, serial no. etc)
							foreach (var input in inputs.Where(x => x.Enable))
							{
								var dbInput = new DbInput
								{
									DeviceId = device.Id,
									InputNo = (short)(input.InputNum + 1),
									Name = input.InputName,
									Type = (InputType)input.InputType,
									ActiveName = input.InputActiveName,
									PassiveName = input.InputPassiveName,
									AlarmZoneMinValue = input.AlarmZone.UserMinVal,
									AlarmZoneMaxValue = input.AlarmZone.UserMaxVal,
									AlarmZoneRangeType = (RangeType)input.AlarmZone.RangeType
								};

								db.Insert(dbInput);
							}

							// update hash
							db.GetTable<DbDevice>()
								.Where(x => x.Id == device.Id)
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

		private async Task<IList<InputsInputNum>> GetInputs(Auth auth)
		{
			try
			{
				_status = "GetInputsInitial";

				var inputsInitialResult = await _gateway.GetInputsInitial(auth);

				var result = new List<InputsInputNum>();

				for (var inputNum = 0; inputNum < inputsInitialResult.InputsInitial.InCount; inputNum++)
				{
					_status = "GetInputsInputNum " + inputNum;

					var inputNumResult = await _gateway.GetInputsInputNum(auth, inputNum);

					result.Add(inputNumResult.InputsInputNum);
				}

				return result;
			}
			finally
			{
				_status = null;
			}
		}
	}

}