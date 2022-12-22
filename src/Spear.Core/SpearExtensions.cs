using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Spear.Core
{
    public static class SpearExtensions
    {
        /// <summary> 服务命名 </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
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

        private static readonly ConcurrentDictionary<MethodInfo, string> RouteCache =
            new ConcurrentDictionary<MethodInfo, string>();

        /// <summary> 获取服务主键 </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string ServiceKey(this MethodInfo method)
        {
            if (RouteCache.TryGetValue(method, out var route))
                return route;

            route = $"{method.DeclaringType?.Name}/{method.Name}".ToLower();
            RouteCache.TryAdd(method, route);

            return route;
        }
    }
}
