using System.Threading;
using System.Threading.Tasks;
using CMon.Requests;
using CMon.Models;
using CMon.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CMon.Controllers.Apis
{
	[Authorize]
	[Route("api/[controller]")]
	public class DeviceController : Controller
	{
		private readonly IInputValueProvider _valueProvider;
		private readonly IIdentityProvider _identityProvider;
		private readonly IBackgroundJob _job;
		private readonly IMediator _mediator;

		public DeviceController(IInputValueProvider valueProvider, IIdentityProvider identityProvider, IBackgroundJob job, IMediator mediator)
		{
			_valueProvider = valueProvider;
			_identityProvider = identityProvider;
			_job = job;
			_mediator = mediator;
		}

		// GET: api/device/values
		[HttpGet("values")]
		public DeviceStatistic GetValues(long deviceId, string from, string to)
		{
			var request = new InputValueRequest { DeviceId = deviceId, BeginDate = from, EndDate = to };

			return _valueProvider.GetValues(request);
		}

		[HttpPost("refresh")]
		public void Refresh(long deviceId)
		{
			_job.Enqueue<IMediator>(x => x.Send(
				new RefreshDevice { DeviceId = deviceId, UserName = _identityProvider.GetUserName() },
				CancellationToken.None));
		}

		[HttpPost("status")]
		public async Task<string> GetStatus(long deviceId)
		{
			var command = new GetDeviceStatus { DeviceId = deviceId };

			return await _mediator.Send(command);
		}

		// GET api/values/5
		/*[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}*/

		// POST api/values
		[HttpPost]
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
