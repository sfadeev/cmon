using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMon.Commands;
using CMon.Entities;
using CMon.Models;
using CMon.Models.Ccu;
using CMon.Queries;
using LinqToDB;
using Montr.Core;

namespace CMon.Services.CommandHandlers
{
	public class RefreshDeviceCommandHandler : ICommandHandler<RefreshDevice, bool>
	{
		private readonly IIdentityProvider _identityProvider;
		private readonly IQueryDispatcher _queryDispatcher;
		private readonly IDbConnectionFactory _connectionFactory;
		private readonly ICcuGateway _gateway;
		private readonly Sha1Hasher _hasher;

		public RefreshDeviceCommandHandler(IIdentityProvider identityProvider,
			IQueryDispatcher queryDispatcher,
			IDbConnectionFactory connectionFactory, ICcuGateway gateway, Sha1Hasher hasher)
		{
			_identityProvider = identityProvider;
			_queryDispatcher = queryDispatcher;
			_connectionFactory = connectionFactory;
			_gateway = gateway;
			_hasher = hasher;
		}

		public async Task<bool> Execute(RefreshDevice command)
		{
			if (_identityProvider.IsAuthenticated == false)
				throw new InvalidOperationException("User should be authenticated to add devices.");

			var device = _queryDispatcher.Dispatch<GetContractDevice, Device>(
				new GetContractDevice { DeviceId = command.DeviceId, WithAuth = true });

			if (device != null)
			{
				var inputs = await GetInputs(device.Auth);

				var hash = _hasher.ComputeHash(inputs);

				if (device.Hash == null || device.Hash.SequenceEqual(hash) == false)
				{
					var userName = _identityProvider.GetUserName();

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
								.Set(x => x.ModifiedBy, userName)
								.Update();

							// todo: add operations log

							transaction.Commit();

							return true;
						}
					}
				}
			}

			return false;
		}

		private async Task<IList<InputsInputNum>> GetInputs(Auth auth)
		{
			var inputsInitialResult = await _gateway.GetInputsInitial(auth);

			var result = new List<InputsInputNum>();

			for (var inputNum = 0; inputNum < inputsInitialResult.InputsInitial.InCount; inputNum++)
			{
				var inputNumResult = await _gateway.GetInputsInputNum(auth, inputNum);

				result.Add(inputNumResult.InputsInputNum);
			}

			return result;
		}
	}
}