using CMon.ViewModels.Page;
using Microsoft.AspNetCore.Mvc;

namespace CMon.Controllers
{
    public class PageController : Controller
    {
        public IActionResult Index(string id)
        {
	        var model = new PageViewModel { Title = id };
			
			return View(model);
        }
    }
}
