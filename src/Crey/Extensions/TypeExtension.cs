using System.ComponentModel;

namespace Crey.Extensions;

internal static class TypeExtension
{
    public static bool IsNullableType(this Type type)
    {
        return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static Type GetUnNullableType(this Type type)
    {
        if (!type.IsNullableType()) return type;
        var nullableConverter = new NullableConverter(type);
        return nullableConverter.UnderlyingType;
    }
}
