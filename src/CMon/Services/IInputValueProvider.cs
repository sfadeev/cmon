using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Models;
using DaleNewman;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CMon.Services
{
	public interface IInputValueProvider
    {
	    Task<DeviceStatistic> GetValues(InputValueRequest request, CancellationToken token);
    }

	public class InputValueOptions
	{
		public int MaxDataPoints { get; set; }  = 500;

		public int MinGroupByMinutes { get; set; }  = 1;

		public int MaxGroupByMinutes { get; set; }  = 59;

		public int? GroupByMinutes { get; set; }
	}

	public class InputValueRequest
	{
		public long DeviceId { get; set; }

		public string BeginDate { get; set; }

		public string EndDate { get; set; }
	}

	public class DefaultInputValueProvider : IInputValueProvider
	{
		private readonly ILogger<DefaultInputValueProvider> _logger;
		private readonly IConfiguration _configuration;

		public DefaultInputValueProvider(ILogger<DefaultInputValueProvider> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		private DbConnection CreateConnection()
		{
			return new DbConnection(_configuration.GetConnectionString("DefaultConnection"));
		}
		
		public async Task<DeviceStatistic> GetValues(InputValueRequest request, CancellationToken token)
		{
			var options = new InputValueOptions();

			var beginDate = DateMath.Parse(request.BeginDate); 
			var endDate = DateMath.Parse(request.EndDate);

			var groupByMinutes = options.GroupByMinutes ?? (int) endDate.Subtract(beginDate).TotalMinutes / options.MaxDataPoints;

			groupByMinutes = Math.Max(Math.Min(
					groupByMinutes, options.MaxGroupByMinutes), options.MinGroupByMinutes);

			_logger.LogDebug("GetValues [{deviceId}] {beginDate} - {endDate}, (group by {groupByMinutes} min)",
				request.DeviceId, beginDate, endDate, groupByMinutes);
			
			using (var db = CreateConnection())
			{
				var table = db.GetTable<DbInputValue>();

				var q1 = from v in table
					where v.DeviceId == request.DeviceId
					      && v.CreatedAt > beginDate && v.CreatedAt <= endDate
					select new
					{
						v.InputNum,
						v.CreatedAt.Year,
						v.CreatedAt.Month,
						v.CreatedAt.Day,
						v.CreatedAt.Hour,
						Minute = Sql.Convert<int, int>(v.CreatedAt.Minute / groupByMinutes) * groupByMinutes,
						v.Value
					};

				var q2 = from v in q1
					select new
					{
						v.InputNum,
						v.Year,
						v.Month,
						v.Day,
						v.Hour,
						Minute = v.Minute > 59 ? 59 : v.Minute,
						v.Value
					};

				// var arr2 = q2.ToArray();

				var q3 = from v in q2
					group v by new
					{
						v.InputNum,
						v.Year,
						v.Month,
						v.Day,
						v.Hour,
						v.Minute
					}
					into g
					select new
					{
						g.Key.InputNum,
						Period = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day, g.Key.Hour, g.Key.Minute, 0, DateTimeKind.Utc),
						Avg = g.Average(x => x.Value),
						Min = g.Min(x => x.Value),
						Max = g.Max(x => x.Value)
					};

				// var sql = q3.ToString();

				// var arr3 = q3.OrderBy(x => x.InputNum).ThenBy(x => x.Period).ToArray();

				var lookup = q3.OrderBy(x => x.InputNum).ThenBy(x => x.Period).ToLookup(x => x.InputNum, x => x);

				var dbInput = await db.GetTable<DbInput>().Where(x => x.DeviceId == request.DeviceId).ToListAsync(token);
				
				var inputs = lookup
					.Select(i => new InputStatistic
					{
						InputNo = i.Key,
						Name = dbInput.FirstOrDefault(x => x.InputNo == i.Key)?.Name ?? "CCUxxx",
						Values = i.Select(x =>
							new InputPeriodValue
							{
								Period = x.Period,
								Avg = x.Avg,
								Min = x.Min,
								Max = x.Max
							}).ToList()
					}).ToList();

				return new DeviceStatistic
				{
					BeginDate = beginDate,
					EndDate = endDate,
					Inputs = inputs
				};
			}
		}
	}
}
