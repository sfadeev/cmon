using System.Threading;
using System.Threading.Tasks;
using CMon.Requests;
using CMon.ViewModels.Device;
using MediatR;

namespace CMon.Services.RequestHandlers
{
	public class GetDeviceBlocksRequestHandler : IRequestHandler<GetDeviceBlocks, GetDeviceBlocks.Result>
	{
		public Task<GetDeviceBlocks.Result> Handle(GetDeviceBlocks request, CancellationToken cancellationToken)
		{
			return Task.FromResult(new GetDeviceBlocks.Result
			{
				Blocks = new[]
				{
					new BlockViewModel { Type = "events", Name = "Events" },
					new BlockViewModel { Type = "single", Name = "1" },
					new BlockViewModel { Type = "single", Name = "2" },
					new BlockViewModel { Type = "single", Name = "3" },
					new BlockViewModel { Type = "single", Name = "4" },
					new BlockViewModel { Type = "single", Name = "5" },
					new BlockViewModel { Type = "single", Name = "6" },
					new BlockViewModel { Type = "single", Name = "255" }
				}
			});
		}
	}
}