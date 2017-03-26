﻿using System.Threading.Tasks;
using CMon.Commands;
using CMon.Queries;
using CMon.ViewModels.Device;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core;

namespace CMon.Controllers
{
	[Authorize]
	public class DeviceController : Controller
	{
		private readonly IQueryDispatcher _queryDispatcher;
		private readonly ICommandDispatcher _commandDispatcher;

		public DeviceController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
		{
			_queryDispatcher = queryDispatcher;
			_commandDispatcher = commandDispatcher;
		}

		public IActionResult Index(long id)
		{
			var model = new DeviceViewModel
			{
				Id = id,
				QuickRanges = new[]
				{
					new[]
					{
						new DateRange { Name = "Last 30 minutes", From = "now-30m", To = "now" },
						new DateRange { Name = "Last 1 hour", From = "now-1h", To = "now" },
						new DateRange { Name = "Last 2 hours", From = "now-2h", To = "now" },
						new DateRange { Name = "Last 4 hours", From = "now-4h", To = "now" },
						new DateRange { Name = "Last 8 hours", From = "now-8h", To = "now" },
						new DateRange { Name = "Last 12 hours", From = "now-12h", To = "now" },
						new DateRange { Name = "Last 24 hours", From = "now-24h", To = "now" },
					},
					new[]
					{
						// new DateRange { Name = "Today", From = "now/d", To = "now/d" },
						new DateRange { Name = "Today so far", From = "now/d", To = "now" },
						// new DateRange { Name = "This week", From = "now/w", To = "now/w" },
						new DateRange { Name = "This week so far", From = "now/w", To = "now" },
						// new DateRange { Name = "This month", From = "now/M", To = "now/M" },
						// new DateRange { Name = "This year", From = "now/y", To = "now/y" },
					}
				}
			};

			return View(model);
		}

		public IActionResult List()
		{
			var query = new GetContractDeviceList();

			var model = _queryDispatcher.Dispatch<GetContractDeviceList, DeviceListViewModel>(query);

			return View(model);
		}

		public IActionResult Add()
		{
			var model = new AddDeviceViewModel();

			return View(model);
		}

		// todo: move to api
		[HttpPost]
		public async Task<IActionResult> Add(AddDeviceViewModel model)
		{
			if (ModelState.IsValid)
			{
				var command = new AddDevice
				{
					Name = model.Name,
					Imei = model.Imei,
					Username = model.Username,
					Password = model.Password
				};

				var deviceId = await _commandDispatcher.Dispatch<AddDevice, long>(command);

				return Redirect(Url.Action("Index", "Device", new { deviceId }));
			}

			return View(model);
		}
	}
}
