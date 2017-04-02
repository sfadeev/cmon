using System.Threading.Tasks;
using CMon.Commands;
using CMon.Models;
using CMon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CMon.Controllers.Apis
{
	[Authorize]
	[Route("api/[controller]")]
	public class DeviceController : Controller
	{
		private readonly IInputValueProvider _valueProvider;
		private readonly IBackgroundJob _backgroundJob;
		private readonly IQueryDispatcher _queryDispatcher;
		private readonly ICommandDispatcher _commandDispatcher;

		public DeviceController(IInputValueProvider valueProvider, IBackgroundJob backgroundJob,
			IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
		{
			_valueProvider = valueProvider;
			_backgroundJob = backgroundJob;
			_queryDispatcher = queryDispatcher;
			_commandDispatcher = commandDispatcher;
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
			var command = new RefreshDevice { DeviceId = deviceId };

			_backgroundJob.Enqueue<ICommandDispatcher>(x => x.Dispatch<RefreshDevice, bool>(command));
			// return await _commandDispatcher.Dispatch<RefreshDevice, bool>(command);
		}

		[HttpPost("status")]
		public async Task<string> GetStatus(long deviceId)
		{
			var command = new GetDeviceStatus { DeviceId = deviceId };

			return await _commandDispatcher.Dispatch<GetDeviceStatus, string>(command);
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
