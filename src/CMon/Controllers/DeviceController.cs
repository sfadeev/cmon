using System.Threading.Tasks;
using CMon.Commands;
using CMon.ViewModels.Device;
using Microsoft.AspNetCore.Mvc;
using Montr.Core;

namespace CMon.Controllers
{
	public class DeviceController : Controller
	{
		private readonly ICommandDispatcher _commandDispatcher;

		public DeviceController(ICommandDispatcher commandDispatcher)
		{
			_commandDispatcher = commandDispatcher;
		}

		public IActionResult Index(long deviceId)
		{
			var model = new DeviceViewModel
			{
				Id = deviceId,
				QuickRanges = new[]
				{
					new[]
					{
						new DateRange { Name = "Last 5 minutes", From = "now-5m", To = "now" },
						new DateRange { Name = "Last 15 minutes", From = "now-15m", To = "now" },
						new DateRange { Name = "Last 30 minutes", From = "now-30m", To = "now" },
						new DateRange { Name = "Last 1 hour", From = "now-1h", To = "now" },
						new DateRange { Name = "Last 3 hours", From = "now-3h", To = "now" },
						new DateRange { Name = "Last 6 hours", From = "now-6h", To = "now" },
						new DateRange { Name = "Last 12 hours", From = "now-12h", To = "now" },
						new DateRange { Name = "Last 24 hours", From = "now-24h", To = "now" },
					},
					new[]
					{
						new DateRange { Name = "Today", From = "now/d", To = "now/d" },
						new DateRange { Name = "Today so far", From = "now/d", To = "now" },
						new DateRange { Name = "This week", From = "now/w", To = "now/w" },
						new DateRange { Name = "This week so far", From = "now/w", To = "now" },
						new DateRange { Name = "This month", From = "now/M", To = "now/M" },
						new DateRange { Name = "This year", From = "now/y", To = "now/y" },
					}
				}
			};

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
