using System.Linq;
using CMon.Entities;
using CMon.Queries;
using CMon.ViewModels.Device;
using Montr.Core;

namespace CMon.Services.QueryHandler
{
	public class GetContractDeviceListQueryHandler : IQueryHandler<GetContractDeviceList, DeviceListViewModel>
	{
		private readonly IIdentityProvider _identityProvider;
		private readonly IDbConnectionFactory _connectionFactory;

		public GetContractDeviceListQueryHandler(IIdentityProvider identityProvider,
			IDbConnectionFactory connectionFactory)
		{
			_identityProvider = identityProvider;
			_connectionFactory = connectionFactory;
		}

		public DeviceListViewModel Retrieve(GetContractDeviceList query)
		{
			var userName = _identityProvider.GetUserName();

			using (var db = _connectionFactory.GetConection())
			{
				var dbContract = (
						from cu in db.GetTable<DbContractUser>()
						join c in db.GetTable<DbContract>() on cu.ContractId equals c.Id
						where cu.UserName == userName
						select c)
					.SingleOrDefault();

				if (dbContract != null)
				{
					var devices = (
							from d in db.GetTable<DbDevice>()
							where d.ContractId == dbContract.Id
							select new DeviceViewModel
							{
								Id = d.Id,
								Name = d.Name,
								Imei = d.Imei
							})
						.ToList();

					return new DeviceListViewModel
					{
						Items = devices
					};
				}

				return null;
			}
		}
	}
}