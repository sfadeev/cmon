using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMon.Models;
using CMon.Requests;
using CMon.ViewModels.Device;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class GetDeviceBlocksRequestHandler : IRequestHandler<GetDeviceBlocks, GetDeviceBlocks.Result>
	{
		private readonly IMediator _mediator;

		public GetDeviceBlocksRequestHandler(IMediator mediator)
		{
			_mediator = mediator;
		}

		public async Task<GetDeviceBlocks.Result> Handle(GetDeviceBlocks request, CancellationToken cancellationToken)
		{
			var device = await _mediator.Send(
				new GetDevice { DeviceId = request.DeviceId, UserName = request.UserName, WithAuth = false }, cancellationToken);

			var blocks = new List<BlockViewModel>
			{
				new BlockViewModel { Type = "events", Name = "События" }
			};

			blocks.AddRange(
				device.Config.Inputs
					.Where(x => x.Type > InputType.Analog)
					.Select(x =>
						new BlockViewModel
						{
							Type = "single",
							Name = x.Name,
							InputNo = x.InputNo
						}));

			blocks.Add(
				new BlockViewModel { Type = "single", Name = device.Name, InputNo = CcuDeviceManager.BoardTemp }
			);

			return new GetDeviceBlocks.Result { Blocks = blocks };
		}
	}
}