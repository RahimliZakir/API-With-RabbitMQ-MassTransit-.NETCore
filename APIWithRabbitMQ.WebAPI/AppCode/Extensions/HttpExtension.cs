using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace APIWithRabbitMQ.WebAPI.AppCode.Extensions
{
    public static partial class Extension
    {
        public static HttpContext GetHttpContext(this IActionContextAccessor ctx)
        {
            HttpContext httpContext = ctx.ActionContext.HttpContext;

            return httpContext;
        }

        public static string GetScheme(this IActionContextAccessor ctx)
        {
            string scheme = ctx.ActionContext.HttpContext.Request.Scheme;

            return scheme;
        }

        public static HostString GetHost(this IActionContextAccessor ctx)
        {
            HostString host = ctx.ActionContext.HttpContext.Request.Host;

            return host;
        }
    }
}
