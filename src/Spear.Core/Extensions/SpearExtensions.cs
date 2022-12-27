using System.Reflection;

namespace Spear.Core.Extensions
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

        /// <summary> 获取服务主键 </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string ServiceKey(this MethodInfo method)
        {
            return $"{method.DeclaringType?.Name}/{method.Name}".ToLower();
        }
    }
}
