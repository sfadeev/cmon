using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Models;
using CMon.Requests;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class GetContractRequestHandler : IRequestHandler<GetContract, Contract>
	{
		private readonly IDbConnectionFactory _connectionFactory;

		public GetContractRequestHandler(IDbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public Task<Contract> Handle(GetContract request, CancellationToken cancellationToken)
		{
			using (var db = _connectionFactory.GetConection())
			{
				var dbContract = (
						from cu in db.GetTable<DbContractUser>()
						join c in db.GetTable<DbContract>() on cu.ContractId equals c.Id
						where cu.UserName == request.UserName
						select c)
					.SingleOrDefault();

				if (dbContract != null)
				{
					var result = new Contract
					{
						Id = dbContract.Id
					};

					result.Devices = db.GetTable<DbDevice>()
						.Where(x => x.ContractId == dbContract.Id)
						.Select(x => new Device
						{
							Id = x.Id,
							Name = x.Name,
							Imei = x.Imei,
							Hash = x.Hash
						})
						.ToList();

					return Task.FromResult(result);
				}
			}

			return Task.FromResult<Contract>(null);
		}
	}
}