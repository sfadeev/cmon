using CMon.Models;
using MediatR;

namespace CMon.Requests
{
	public class GetContract : IRequest<Contract>
	{
		public string UserName { get; set; }
	}
}