using System.Threading;
using System.Threading.Tasks;
using CMon.Requests;
using DaleNewman;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class GetDeviceEventsRequestHandler : IRequestHandler<GetDeviceEvents, GetDeviceEvents.Result>
	{
		private readonly IDeviceRepository _deviceRepository;
		private readonly DeviceEventDisplayResolver _displayResolver;

		public GetDeviceEventsRequestHandler(IDeviceRepository deviceRepository, DeviceEventDisplayResolver displayResolver)
		{
			_deviceRepository = deviceRepository;
			_displayResolver = displayResolver;
		}

		public Task<GetDeviceEvents.Result> Handle(GetDeviceEvents request, CancellationToken cancellationToken)
		{
			var result = new GetDeviceEvents.Result
			{
				BeginDate = DateMath.Parse(request.BeginDate),
				EndDate = DateMath.Parse(request.EndDate)
			};

			result.Items = _deviceRepository.LoadEvents(request.DeviceId, result.BeginDate, result.EndDate);

			foreach (var item in result.Items)
			{
				_displayResolver.Resolve(item);
			}

			// test
			/*foreach (var type in Enum.GetNames(typeof(EventType)))
			{
				var random = new Random();
				var item = new DeviceEvent
				{
					Id = random.Next(1000000),
					EventType = type,
					ExternalId = random.Next(10000),
					CreatedAt = DateTime.Now.Subtract(TimeSpan.FromHours(random.NextDouble())),
					Info = new DeviceEventInformation
					{
						Partition = 1,
						Partitions = new[] { 1, 2, 3, 4 },
						Number = 2,
						Source = new ArmSource
						{
							Type = ArmSourceType.Shell.ToString(),
							Key = "1234567890",
							KeyName = "key name",
							Phone = "+79801234567"
						},
						ErrorCode = 9,
						UserName = "user_name"
					}
				};
				_displayResolver.Resolve(item);
				result.Items.Add(item);
			}*/

			return Task.FromResult(result);
		}
	}
}