using CMon.ViewModels.Device;
using Microsoft.AspNetCore.Mvc;

namespace CMon.Controllers
{
    public class DeviceController : Controller
    {
        public IActionResult Index(long deviceId)
        {
			var model = new DeviceViewModel { Id = deviceId };

            return View(model);
        }
    }
}
