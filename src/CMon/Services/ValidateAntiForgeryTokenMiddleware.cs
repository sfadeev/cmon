using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CMon.Services
{
    public class ValidateAntiForgeryTokenMiddleware
    {
		private readonly RequestDelegate _next;
		private readonly IAntiforgery _antiforgery;

		public ValidateAntiForgeryTokenMiddleware(RequestDelegate next, IAntiforgery antiforgery)
		{
			_next = next;
			_antiforgery = antiforgery;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			if (httpContext.Request.Method.ToUpper() == HttpMethod.Post.Method)
			{
				await _antiforgery.ValidateRequestAsync(httpContext);
			}

			await _next(httpContext);
		}
	}

	public static class ValidateAntiForgeryTokenMiddlewareExtensions
	{
		public static IApplicationBuilder UseValidateAntiForgeryToken(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ValidateAntiForgeryTokenMiddleware>();
		}
	}
}
