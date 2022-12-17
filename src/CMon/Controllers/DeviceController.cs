using CMon.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CMon.Controllers
{
    public class DeviceController : Controller
    {
        [Route("d/{deviceId}")]
        public IActionResult Index(long deviceId)
        {
			var model = new DeviceViewModel { Id = deviceId };

            return View(model);
        }
    }
}
