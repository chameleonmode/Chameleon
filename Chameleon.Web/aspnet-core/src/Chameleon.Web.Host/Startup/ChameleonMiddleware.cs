using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Chameleon.Web.Host.Startup
{
    public class ChameleonMiddleware
    {
        private readonly RequestDelegate _next;

        public ChameleonMiddleware(
            RequestDelegate next
            )
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            HandleIgnoreHeaders(httpContext);
            SetAuthorizationHeaderFromCookie(httpContext);
            await _next(httpContext);
        }

        private void HandleIgnoreHeaders(HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("X-Ignore-Headers"))
            {
                httpContext.Request.Headers.Clear();
            }
        }


        private void SetAuthorizationHeaderFromCookie(HttpContext httpContext)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                return;
            }

            var authorizationCookie = httpContext.Request.Cookies["Abp.AuthToken"];
            if (string.IsNullOrEmpty(authorizationCookie))
            {
                return;
            }

            httpContext.Request.Headers["Authorization"] = "Bearer " + authorizationCookie;
        }
    }
}
