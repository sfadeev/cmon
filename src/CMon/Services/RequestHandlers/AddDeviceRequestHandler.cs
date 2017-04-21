using System;
using System.Linq;
using CMon.Entities;
using CMon.Models;
using CMon.Requests;
using LinqToDB;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class AddDeviceRequestHandler : IRequestHandler<AddDevice, long>
	{
		private readonly IIdentityProvider _identityProvider;
		private readonly IDbConnectionFactory _connectionFactory;

		public AddDeviceRequestHandler(IIdentityProvider identityProvider,
			IDbConnectionFactory connectionFactory)
		{
			_identityProvider = identityProvider;
			_connectionFactory = connectionFactory;
		}

		public long Handle(AddDevice command)
		{
			if (_identityProvider.IsAuthenticated == false)
				throw new InvalidOperationException("User should be authenticated to add devices.");

			var now = DateTime.UtcNow;

			var userName = _identityProvider.GetUserName();

			var tarifLimit = new { MaxDeviceCount = 20 };

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
					var deviceCount = db.GetTable<DbDevice>().Count(x => x.ContractId == contractUser.ContractId);
					if (deviceCount >= tarifLimit.MaxDeviceCount) throw new InvalidOperationException("Tarif limit of devices exceeded.");

					var device = new DbDevice
					{
						ContractId = contractUser.ContractId,
						Status = DeviceStatus.None,
						Name = command.Name,
						Imei = command.Imei,
						Username = command.Username,
						Password = command.Password,
						CreatedAt = now,
						CreatedBy = userName
					};

					var deviceId = (long)db.InsertWithIdentity(device);

					// todo: add operations log

					transaction.Commit();

					return deviceId;
				}
			}
		}
	}
}
