using System.Collections.Generic;
using CMon.ViewModels.Device;
using MediatR;

namespace CMon.Requests
{
	public class GetDeviceBlocks : AbstractDeviceRequest, IRequest<GetDeviceBlocks.Result>
	{
		public class Result
		{
			public IList<BlockViewModel> Blocks { get; set; }
		}
	}
}