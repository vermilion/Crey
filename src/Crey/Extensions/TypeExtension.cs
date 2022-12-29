using System.ComponentModel;

namespace Crey.Extensions;

/// <summary>
/// 类型<see cref="Type"/>辅助扩展方法类
/// </summary>
public static class TypeExtension
{
    private static readonly List<Type> SimpleTypes = new List<Type>
    {
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(bool),
        typeof(string),
        typeof(char),
        typeof(Guid),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(byte[])
    };

    /// <summary>
    /// 判断类型是否为Nullable类型
    /// </summary>
    /// <param name="type"> 要处理的类型 </param>
    /// <returns> 是返回True，不是返回False </returns>
    public static bool IsNullableType(this Type type)
    {
        return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// 通过类型转换器获取Nullable类型的基础类型
    /// </summary>
    /// <param name="type"> 要处理的类型对象 </param>
    /// <returns> </returns>
    public static Type GetUnNullableType(this Type type)
    {
        if (!type.IsNullableType()) return type;
        var nullableConverter = new NullableConverter(type);
        return nullableConverter.UnderlyingType;
    }

    /// <summary> 是否是简单类型 </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSimpleType(this Type type)
    {
        var actualType = type.GetUnNullableType();
        return SimpleTypes.Contains(actualType);
    }

}
