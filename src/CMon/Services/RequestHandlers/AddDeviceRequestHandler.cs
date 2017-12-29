using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		public Task<long> Handle(AddDevice command, CancellationToken cancellationToken)
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
					long contractId;

					var contractUser = db.GetTable<DbContractUser>()
						.SingleOrDefault(x => x.UserName == userName);

					if (contractUser == null)
					{
						var contract = new DbContract
						{
							CreatedAt = now,
							CreatedBy = userName
						};

						contractId = (long)db.InsertWithIdentity(contract);

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

						contractId = contractUser.ContractId;

						db.GetTable<DbContract>()
							.Where(x => x.Id == contractId)
							.Set(x => x.ModifiedAt, now)
							.Set(x => x.ModifiedBy, userName)
							.Update();
					}

					// todo: use tarif limits
					var deviceCount = db.GetTable<DbDevice>().Count(x => x.ContractId == contractId);

					// todo: validation before add
					if (deviceCount >= tarifLimit.MaxDeviceCount)
						throw new InvalidOperationException("Tarif limit of devices exceeded.");

					var device = new DbDevice
					{
						ContractId = contractId,
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

					return Task.FromResult(deviceId) ;
				}
			}
		}
	}
}
