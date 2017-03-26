using System.Linq;
using CMon.Entities;
using CMon.Models;
using CMon.Models.Ccu;
using CMon.Queries;
using Montr.Core;

namespace CMon.Services.QueryHandler
{
	public class GetContractDeviceQueryHandler : IQueryHandler<GetContractDevice, Device>
	{
		private readonly IIdentityProvider _identityProvider;
		private readonly IDbConnectionFactory _connectionFactory;

		public GetContractDeviceQueryHandler(IIdentityProvider identityProvider,
			IDbConnectionFactory connectionFactory)
		{
			_identityProvider = identityProvider;
			_connectionFactory = connectionFactory;
		}

		public Device Retrieve(GetContractDevice query)
		{
			var userName = _identityProvider.GetUserName();

			using (var db = _connectionFactory.GetConection())
			{
				var dbDevice = (
						from cu in db.GetTable<DbContractUser>()
						join d in db.GetTable<DbDevice>() on cu.ContractId equals d.ContractId
						where cu.UserName == userName && d.Id == query.DeviceId
						select d)
					.SingleOrDefault();

				if (dbDevice != null)
				{
					var device = new Device
					{
						Id = dbDevice.Id,
						Name = dbDevice.Name,
						Imei = dbDevice.Imei
					};

					if (query.WithAuth)
					{
						device.Auth = new Auth
						{
							Imei = dbDevice.Imei,
							Username = dbDevice.Username,
							Password = dbDevice.Password
						};
					}

					return device;
				}

				return null;
			}
		}
	}
}