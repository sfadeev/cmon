using Microsoft.AspNetCore.Mvc;

namespace CMon.Web.Controllers
{
	public class ChangeLogController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}