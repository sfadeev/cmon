using CMon.Web.ViewModels.Page;
using Microsoft.AspNetCore.Mvc;

namespace CMon.Web.Controllers
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
