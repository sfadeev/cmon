using System.Threading;
using System.Threading.Tasks;
using CMon.Requests;
using CMon.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMon.Controllers.Apis
{
	[Authorize]
	[Route("api/[controller]")]
	public class DeviceController : Controller
	{
		private readonly IIdentityProvider _identityProvider;
		private readonly IBackgroundJob _job;
		private readonly IMediator _mediator;

		public DeviceController(IIdentityProvider identityProvider, IBackgroundJob job, IMediator mediator)
		{
			_identityProvider = identityProvider;
			_job = job;
			_mediator = mediator;
		}

		[HttpGet("values")]
		public async Task<GetDeviceInputs.Result> GetValues(long deviceId, string from, string to)
		{
			return await _mediator.Send(new GetDeviceInputs
			{
				DeviceId = deviceId,
				UserName = _identityProvider.GetUserName(),
				BeginDate = from,
				EndDate = to
			});
		}

		[HttpPost("events")]
		public async Task<GetDeviceEvents.Result> GetEvents(long deviceId, string from, string to)
		{
			return await _mediator.Send(new GetDeviceEvents
			{
				DeviceId = deviceId,
				UserName = _identityProvider.GetUserName(),
				BeginDate = from,
				EndDate = to
			});
		}

		[HttpPost("refresh")]
		public void Refresh(long deviceId)
		{
			_job.Enqueue<IMediator>(x => x.Send(new RefreshDevice
			{
				DeviceId = deviceId,
				UserName = _identityProvider.GetUserName()
			}, CancellationToken.None));
		}

		[HttpPost("status")]
		public async Task<string> GetStatus(long deviceId)
		{
			return await _mediator.Send(new GetDeviceStatus
			{
				DeviceId = deviceId
				// UserName = _identityProvider.GetUserName()
			});
		}
	}
}
