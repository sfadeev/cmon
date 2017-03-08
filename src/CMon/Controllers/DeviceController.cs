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
			var model = new DeviceViewModel { Id = deviceId };

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
