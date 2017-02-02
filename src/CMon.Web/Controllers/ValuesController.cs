using System;
using CMon.Models;
using CMon.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CMon.Web.Controllers
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
		public DeviceStatistic GetValues(long deviceId, int h)
		{
			var endDate = DateTime.UtcNow;
			var beginDate = endDate.AddHours(-h);

			return _valueProvider.GetValues(deviceId, beginDate, endDate, 1);
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
