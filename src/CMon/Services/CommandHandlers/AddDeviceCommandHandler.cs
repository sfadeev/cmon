using System;
using System.Linq;
using CMon.Commands;
using CMon.Entities;
using CMon.Models;
using LinqToDB;
using Montr.Core;

namespace CMon.Services.CommandHandlers
{
	public class AddDeviceCommandHandler : ICommandHandler<AddDevice, long>
	{
		private readonly IIdentityProvider _identityProvider;
		private readonly IDbConnectionFactory _connectionFactory;

		public AddDeviceCommandHandler(IIdentityProvider identityProvider,
			IDbConnectionFactory connectionFactory)
		{
			_identityProvider = identityProvider;
			_connectionFactory = connectionFactory;
		}

		public long Execute(AddDevice command)
		{
			if (_identityProvider.IsAuthenticated == false)
				throw new InvalidOperationException("User should be authenticated to add devices.");

			var now = DateTime.UtcNow;
			
			var userName = _identityProvider.GetUserName();

			var tarifLimit = new { MaxDeviceCount = 2 };

			using (var db = _connectionFactory.GetConection())
			{
				using (var transaction = db.BeginTransaction())
				{
					var contractUser = db.GetTable<DbContractUser>()
						.SingleOrDefault(x => x.UserName == userName);

					if (contractUser == null)
					{
						var contract = new DbContract
						{
							CreatedAt = now,
							CreatedBy = userName
						};

						var contractId = (long)db.InsertWithIdentity(contract);

						contractUser = new DbContractUser
						{
							ContractId = contractId,
							UserName = userName,
							Role = ContractUserRole.Admin,
							/*CreatedAt = now,
							CreatedBy = userName*/
						};

						db.Insert(contractUser);
					}
					else 
					{
						// todo: check role privileges
						if (contractUser.Role != ContractUserRole.Admin && contractUser.Role != ContractUserRole.Manager)
							throw new InvalidOperationException("User is not authorized to add devices.");

						db.GetTable<DbContract>()
							.Where(x => x.Id == contractUser.ContractId)
							.Set(x => x.ModifiedAt, now)
							.Set(x => x.ModifiedBy, userName)
							.Update();
					}

					// todo: use tarif limits
					var deviceCount = db.GetTable<DbContractDevice>().Count(x => x.ContractId == contractUser.ContractId);
					if (deviceCount >= tarifLimit.MaxDeviceCount) throw new InvalidOperationException("Tarif limit of devices exceeded.");

					var device = new DbDevice
					{
						Imei = command.Imei,
						Username = command.Username,
						Password = command.Password,
						CreatedAt = now,
						CreatedBy = userName
					};

					var deviceId = (long)db.InsertWithIdentity(device);

					db.Insert(new DbContractDevice
					{
						DeviceId = deviceId,
						ContractId = contractUser.ContractId,
						/*CreatedAt = now,
						CreatedBy = userName*/
					});

					// todo: add operations log
					// todo: read device info

					transaction.Commit();

					return deviceId;
				}
			}
		}
	}
}
