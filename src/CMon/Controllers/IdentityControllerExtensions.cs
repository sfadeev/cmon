using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CMon.Controllers
{
    public static class IdentityControllerExtensions
    {
		// https://bitbucket.org/sfadeev/cmon/issues/1/app-redirects-to-account-accessdenied-on
		public static bool DeleteExternalLoginCookie(this Controller controller, IOptions<IdentityOptions> identityOptions)
	    {
			var externalCookieName = identityOptions.Value.Cookies.ExternalCookie.CookieName;

		    if (controller.Request.Cookies[externalCookieName] != null)
		    {
			    controller.Response.Cookies.Delete(externalCookieName);
				return true;
		    }

		    return false;
	    }
	}
}
