using System.Threading;
using System.Threading.Tasks;
using CMon.Models;
using CMon.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMon.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		private readonly IInputValueProvider _valueProvider;

		public ValuesController(IInputValueProvider valueProvider)
		{
			_valueProvider = valueProvider;
		}
		
		[HttpGet]
		public async Task<DeviceStatistic> GetValues(long deviceId, string from, string to, CancellationToken token)
		{
			var request = new InputValueRequest { DeviceId = deviceId, BeginDate = @from, EndDate = to };

			return await _valueProvider.GetValues(request, token);
		}
	}
}
