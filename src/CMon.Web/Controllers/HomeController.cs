using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace CMon.Web.Controllers
{
	public class HomeController : Controller
    {
	    private readonly IStringLocalizer<HomeController> _localizer;

	    public HomeController(IStringLocalizer<HomeController> localizer)
	    {
		    _localizer = localizer;
	    }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

		[HttpPost]
		public IActionResult SetLanguage(string culture, string returnUrl)
		{
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			return LocalRedirect(returnUrl);
		}

		public IActionResult Error()
        {
            return View();
        }
    }
}
