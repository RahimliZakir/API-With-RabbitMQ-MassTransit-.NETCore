using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace APIWithRabbitMQ.WebAPI.AppCode.Extensions
{
    public static partial class Extension
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            ClaimsIdentity identity = principal.Identity as ClaimsIdentity;

            int userId = Convert.ToInt32(identity.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value);

            return userId;
        }

        public static int GetUserId(this IActionContextAccessor ctx)
        {
            return ctx.ActionContext.HttpContext.User.GetUserId();
        }
    }
}
