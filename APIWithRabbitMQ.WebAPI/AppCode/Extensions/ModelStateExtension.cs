using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace APIWithRabbitMQ.WebAPI.AppCode.Extensions
{
    public static partial class Extension
    {
        public static bool IsValid(this IActionContextAccessor ctx)
        {
            return ctx.ActionContext.ModelState.IsValid;
        }
    }
}
