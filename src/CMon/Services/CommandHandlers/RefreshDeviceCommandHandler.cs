using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMon.Commands;
using CMon.Models;
using CMon.Models.Ccu;
using CMon.Queries;
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

				if (hash != device.Hash)
				{
					using (var db = _connectionFactory.GetConection())
					{
						using (var transaction = db.BeginTransaction())
						{
							// remove current inputs

							// insert new inputs

							// update hash

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