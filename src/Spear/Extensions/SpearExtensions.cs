using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spear.Core.Micro.Abstractions;
using Spear.Micro.Abstractions;
using Spear.Micro.Constants;
using Spear.ProxyGenerator.Abstractions;

namespace Spear.Core.Extensions
{
    public static class SpearExtensions
    {
        /// <summary>
        /// Invoke service "OneWay", without waiting for result
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="serviceProvider">Provider instance <see cref="IServiceProvider"/></param>
        /// <param name="action">Action to execute</param>
        /// <returns><see cref="Task"/></returns>
        public static async Task InvokeOneWay<T>(IServiceProvider serviceProvider, Func<T, Task> action)
            where T : class, IMicroService
        {
            await using var scope = serviceProvider.CreateAsyncScope();

            var proxyFactory = scope.ServiceProvider.GetRequiredService<IProxyFactory>();

            var session = scope.ServiceProvider.GetRequiredService<IMicroSession>();
            session.Set(MicroConstants.LongRunning, true);

            var service = proxyFactory.Create<T>();

            await action(service);
        }

        public static string ServiceName(this Assembly assembly)
        {
            var assName = assembly.GetName();
            return $"{assName.Name}_v{assName.Version.Major}";
        }

        public static string TypeName(this Type type)
        {
            var code = Type.GetTypeCode(type);
            if (code != TypeCode.Object && type.BaseType != typeof(Enum))
                return type.FullName;

            return type.AssemblyQualifiedName;
        }

        public static string ServiceKey(this MethodInfo method)
        {
            return $"{method.DeclaringType.Name}/{method.Name}".ToLower();
        }
    }
}
