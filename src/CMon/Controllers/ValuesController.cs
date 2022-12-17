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

		// GET: api/values
		[HttpGet]
		public DeviceStatistic GetValues(long deviceId, string from, string to)
		{
			var request = new InputValueRequest { DeviceId = deviceId, BeginDate = @from, EndDate = to };

			return _valueProvider.GetValues(request);
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
