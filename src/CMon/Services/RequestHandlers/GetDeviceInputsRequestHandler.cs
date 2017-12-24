using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Entities;
using CMon.Models;
using CMon.Requests;
using DaleNewman;
using LinqToDB;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class InputValueOptions
	{
		public int MaxDataPoints { get; set; }  = 500;

		public int MinGroupByMinutes { get; set; }  = 1;

		public int MaxGroupByMinutes { get; set; }  = 59;

		public int? GroupByMinutes { get; set; }
	}

	public class GetDeviceInputsRequestHandler : IRequestHandler<GetDeviceInputs, GetDeviceInputs.Result>
	{
		private readonly IMediator _mediator;
		private readonly IDbConnectionFactory _connectionFactory;

		public GetDeviceInputsRequestHandler(IMediator mediator, IDbConnectionFactory connectionFactory)
		{
			_mediator = mediator;
			_connectionFactory = connectionFactory;
		}

		public async Task<GetDeviceInputs.Result> Handle(GetDeviceInputs request, CancellationToken cancellationToken)
		{
			var options = new InputValueOptions();

			var beginDate = DateMath.Parse(request.BeginDate); 
			var endDate = DateMath.Parse(request.EndDate);

			var groupByMinutes = options.GroupByMinutes ?? (int) endDate.Subtract(beginDate).TotalMinutes / options.MaxDataPoints;

			groupByMinutes = Math.Max(Math.Min(
					groupByMinutes, options.MaxGroupByMinutes), options.MinGroupByMinutes);

			var device = await _mediator.Send(new GetDevice { DeviceId = request.DeviceId, UserName = request.UserName }, cancellationToken);

			using (var db = _connectionFactory.GetConection())
			{
				// var dbInput = db.GetTable<DbInput>().Where(x => x.DeviceId == request.DeviceId).ToList();

				var table = db.GetTable<DbInputValue>();

				var q1 = from v in table
					where v.DeviceId == request.DeviceId && v.CreatedAt > beginDate && v.CreatedAt <= endDate
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

				var inputs = lookup
					.Select(i => new InputStatistic
					{
						InputNo = i.Key,
						Name = device.Config?.Inputs?.FirstOrDefault(x => x.InputNo == i.Key)?.Name ?? device.Name,
						Values = i.Select(x =>
							new InputPeriodValue
							{
								Period = x.Period,
								Avg = x.Avg,
								Min = x.Min,
								Max = x.Max
							}).ToList()
					}).ToList();

				return new GetDeviceInputs.Result
				{
					BeginDate = beginDate,
					EndDate = endDate,
					Inputs = inputs
				};
			}
		}
	}
}
