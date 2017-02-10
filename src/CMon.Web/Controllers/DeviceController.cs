using CMon.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CMon.Web.Controllers
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
