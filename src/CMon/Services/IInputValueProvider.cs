using System;
using System.Linq;
using CMon.Entities;
using CMon.Models;
using LinqToDB;
using Microsoft.Extensions.Options;

namespace CMon.Services
{
	public interface IInputValueProvider
    {
		DeviceStatistic GetValues(long deviceId,  DateTime begin, DateTime end, int groupByMinutes);
    }

	public class DefaultInputValueProvider : IInputValueProvider
	{
		private readonly ConnectionStringOptions _connectionStrings;

		public DefaultInputValueProvider(IOptions<ConnectionStringOptions> connectionStringOptions)
		{
			_connectionStrings = connectionStringOptions.Value;
		}

		public DeviceStatistic GetValues(long deviceId, DateTime begin, DateTime end, int groupByMinutes)
		{
			using (var db = new DbConnection(_connectionStrings.DefaultConnection))
			{
				var dbInput = db.GetTable<DbInput>().Where(x => x.DeviceId == deviceId).ToList();

				var table = db.GetTable<DbInputValue>();

				var q1 = from v in table
					where v.DeviceId == deviceId && v.CreatedAt > begin && v.CreatedAt <= end
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
						Minute = v.Minute == 60 ? 0 : v.Minute,
						v.Value
					};

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

				// var arr = q3.OrderBy(x => x.InputNum).ThenBy(x => x.Period).ToArray();

				var lookup = q3.OrderBy(x => x.InputNum).ThenBy(x => x.Period).ToLookup(x => x.InputNum, x => x);

				var inputs = lookup
					.Select(i => new InputStatistic
					{
						InputNo = i.Key,
						Name = dbInput.FirstOrDefault(x => x.InputNo == i.Key)?.Name ?? "TEMP",
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
					BeginDate = begin,
					EndDate = end,
					Inputs = inputs
				};
			}
		}
	}
}
