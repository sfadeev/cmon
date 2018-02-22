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

			return Task.FromResult(result);
		}
	}
}