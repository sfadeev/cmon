using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using CMon.Entities;
using CMon.Services;
using CMon.Web.Models;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CMon.Web.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		private readonly ConnectionStringOptions _connectionStrings;

		public ValuesController(IOptions<ConnectionStringOptions> connectionStringOptions)
		{
			_connectionStrings = connectionStringOptions.Value;
		}

		// GET: api/values
		[HttpGet]
		public IEnumerable Get(int h)
		{
			long deviceId = 0;

			return new[]
			{
				GetValues(h, deviceId, 0),
				GetValues(h, deviceId, 1),
				GetValues(h, deviceId, CMon.Program.BoardTemp)
			};
		}

		private InputValue[] GetValues(int hours, long deviceId, short inputNum)
		{
			using (var db = new DbConnection(_connectionStrings.DefaultConnection))
			{
				// db.Execute()

				var query = from v in db.GetTable<DbInputValue>()
							where v.DeviceId == deviceId && v.InputNum == inputNum
								&& v.CreatedAt > DateTime.UtcNow.AddHours(-hours)
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
