using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Requests;
using CMon.ViewModels.Device;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CMon.Services.RequestHandlers
{
	public class GetDeviceListRequestHandler : IRequestHandler<GetDeviceList, DeviceListViewModel>
	{
		private readonly IDbConnectionFactory _connectionFactory;
		private readonly IUrlHelperFactory _urlHelperFactory;
		private readonly IActionContextAccessor _actionContextAccessor;
		// private readonly IUrlHelper _urlHelper;

		public GetDeviceListRequestHandler(IDbConnectionFactory connectionFactory,
			IUrlHelperFactory  urlHelperFactory, IActionContextAccessor actionContextAccessor /*, IUrlHelper urlHelper*/)
		{
			_connectionFactory = connectionFactory;
			_urlHelperFactory = urlHelperFactory;
			_actionContextAccessor = actionContextAccessor;
			// _urlHelper = urlHelper;
		}

		public Task<DeviceListViewModel> Handle(GetDeviceList query, CancellationToken cancellationToken)
		{
			using (var db = _connectionFactory.GetConection())
			{
				var dbContract = (
						from cu in db.GetTable<DbContractUser>()
						join c in db.GetTable<DbContract>() on cu.ContractId equals c.Id
						where cu.UserName == query.UserName
						select c)
					.SingleOrDefault();

				if (dbContract != null)
				{
					var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

					var devices = (
							from d in db.GetTable<DbDevice>()
							where d.ContractId == dbContract.Id
							select new DeviceViewModel
							{
								Url = urlHelper.Action("Index", "Device", new { id = d.Id }),
								Id = d.Id,
								Name = d.Name,
								Imei = d.Imei
							})
						.ToList();

					return Task.FromResult(new DeviceListViewModel
					{
						Items = devices
					});
				}

				return Task.FromResult(new DeviceListViewModel
				{
					Items = Enumerable.Empty<DeviceViewModel>().ToList()
				});
			}
		}
	}
}