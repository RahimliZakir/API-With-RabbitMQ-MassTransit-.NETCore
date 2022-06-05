using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace APIWithRabbitMQ.WebAPI.AppCode.Extensions
{
    public static partial class Extension
    {
        public static string[] GetPrincipals<T>(this T item, Type type)
        {
            Type[] types = type.Assembly.GetTypes();

            string[] principals = types
                                  .Where(t => typeof(ControllerBase).IsAssignableFrom(t)
                                  && t.IsDefined(typeof(AuthorizeAttribute), true))
                                  .SelectMany(t => t.GetCustomAttributes<AuthorizeAttribute>())
                                  .Union(types
                                  .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
                                  .SelectMany(type => type.GetMethods())
                                  .Where(method => method.IsPublic
                                  && !method.IsDefined(typeof(NonActionAttribute))
                                  && method.IsDefined(typeof(AuthorizeAttribute)))
                                  .SelectMany(t => t.GetCustomAttributes<AuthorizeAttribute>()))
                                  .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
                                  .SelectMany(a => a.Policy.Split(new[] { ',' },
                                  StringSplitOptions.RemoveEmptyEntries))
                                  .Distinct()
                                  .ToArray();

            return principals;
        }
    }
}
