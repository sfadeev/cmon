using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using CMon.Entities;
using CMon.Models;
using CMon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CMon.Web.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		private readonly IInputValueProvider _valueProvider;
		private readonly ConnectionStringOptions _connectionStrings;

		public ValuesController(IInputValueProvider valueProvider,IOptions<ConnectionStringOptions> connectionStringOptions)
		{
			_valueProvider = valueProvider;
			_connectionStrings = connectionStringOptions.Value;
		}

		// GET: api/values
		[HttpGet("GetValues")]
		public DeviceStatistic GetValues(long deviceId, int h)
		{
			var endDate = DateTime.UtcNow;
			var beginDate = endDate.AddHours(-h);

			return _valueProvider.GetValues(deviceId, beginDate, endDate, 1);
		}

		// GET: api/values
		[HttpGet]
		public IEnumerable Get(long deviceId, int h)
		{
			var beginDate = DateTime.UtcNow.AddHours(-h);

			return new[]
			{
				GetValues(deviceId, 1, beginDate),
				GetValues(deviceId, 2, beginDate),
				GetValues(deviceId, CMon.Program.BoardTemp, beginDate)
			};
		}

		private InputValue[] GetValues(long deviceId, short inputNum, DateTime beginDate)
		{
			using (var db = new DbConnection(_connectionStrings.DefaultConnection))
			{
				// db.Execute()

				var query = from v in db.GetTable<DbInputValue>()
							where v.DeviceId == deviceId && v.InputNum == inputNum
								&& v.CreatedAt > beginDate
							orderby v.CreatedAt descending
							select v;

				var values = query.ToList().Select(x => new InputValue
				{
					InputNum = x.InputNum,
					Date = x.CreatedAt.ToLocalTime().ToString(CultureInfo.InvariantCulture),
					Value = x.Value
				}).ToArray();

				return values;
			}
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
