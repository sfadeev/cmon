using System.Linq;
using CMon.Entities;
using CMon.Models;
using CMon.Models.Ccu;
using CMon.Requests;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class GetDeviceRequestHandler : IRequestHandler<GetDevice, Device>
	{
		private readonly IDbConnectionFactory _connectionFactory;

		public GetDeviceRequestHandler(IDbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public Device Handle(GetDevice query)
		{
			using (var db = _connectionFactory.GetConection())
			{
				var dbDevice = (
						from cu in db.GetTable<DbContractUser>()
						join d in db.GetTable<DbDevice>() on cu.ContractId equals d.ContractId
						where cu.UserName == query.UserName && d.Id == query.DeviceId
						select d)
					.SingleOrDefault();

				if (dbDevice != null)
				{
					var device = new Device
					{
						Id = dbDevice.Id,
						Name = dbDevice.Name,
						Imei = dbDevice.Imei,
						Hash = dbDevice.Hash
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