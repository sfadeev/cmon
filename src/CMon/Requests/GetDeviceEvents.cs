using System;
using System.Collections.Generic;
using CMon.Models;
using MediatR;

namespace CMon.Requests
{
	public class GetDeviceEvents : AbstractDeviceRequest, IRequest<GetDeviceEvents.Result>
	{
		public string BeginDate { get; set; }

		public string EndDate { get; set; }

		public class Result
		{
			public DateTime BeginDate { get; set; }

			public DateTime EndDate { get; set; }

			public IList<DeviceEvent> Items { get; set; }
		}
	}
}