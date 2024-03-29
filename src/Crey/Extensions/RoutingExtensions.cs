﻿using System.Reflection;

namespace Crey.Extensions;

public static class RoutingExtensions
{
    public static string ServiceName(this Assembly assembly)
    {
        var assemblyName = assembly.GetName();
        return $"{assemblyName.Name}_v{assemblyName.Version.Major}";
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
