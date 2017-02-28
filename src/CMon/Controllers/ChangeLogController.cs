using Microsoft.AspNetCore.Mvc;

namespace CMon.Controllers
{
	public class ChangeLogController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}