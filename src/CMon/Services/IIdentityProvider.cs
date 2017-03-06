using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CMon.Services
{
	public interface IIdentityProvider
	{
		bool IsAuthenticated { get; }

		long? GetUserId();

		string GetUserName();
	}

	public class ClaimsIdentityProvider : IIdentityProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ClaimsIdentityProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public bool IsAuthenticated => _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

		public long? GetUserId()
		{
			var identity = _httpContextAccessor.HttpContext.User.Identity;

			if (identity.IsAuthenticated)
			{
				return long.Parse(((ClaimsIdentity)identity).FindFirst(ClaimTypes.NameIdentifier).Value);
			}

			return null;
		}

		public string GetUserName()
		{
			var identity = _httpContextAccessor.HttpContext.User.Identity;

			return identity.IsAuthenticated ? identity.Name : null;
		}
	}
}