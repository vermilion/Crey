using System.Text;

namespace Crey.Extensions;

public static class CommonExtensions
{
    public static T? CastTo<T>(this object obj)
    {
        return obj.CastTo(default(T?));
    }

    public static T? CastTo<T>(this object obj, T def)
    {
        var value = obj.CastTo(typeof(T?));
        if (value == null)
            return def;

        return (T?)value;
    }

    public static object? CastTo(this object obj, Type conversionType)
    {
        if (obj == null)
        {
            return conversionType.IsGenericType ? Activator.CreateInstance(conversionType) : null;
        }

        if (conversionType.IsNullableType())
            conversionType = conversionType.GetUnNullableType();

        try
        {
            if (conversionType == obj.GetType())
                return obj;

            if (conversionType.IsEnum)
            {
                return obj is string s
                    ? Enum.Parse(conversionType, s)
                    : Enum.ToObject(conversionType, obj);
            }

            if (!conversionType.IsInterface && conversionType.IsGenericType)
            {
                var innerType = conversionType.GetGenericArguments()[0];
                var innerValue = obj.CastTo(innerType);
                return Activator.CreateInstance(conversionType, innerValue);
            }

            if (conversionType == typeof(Guid))
            {
                if (Guid.TryParse(obj.ToString(), out var guid))
                    return guid;
                return null;
            }

            if (conversionType == typeof(Version))
            {
                if (Version.TryParse(obj.ToString(), out var version))
                    return version;

                return null;
            }

            return obj is IConvertible 
                ? Convert.ChangeType(obj, conversionType) 
                : obj;
        }
        catch
        {
            return null;
        }
    }

    public static string Format(this Exception ex, bool isHideStackTrace = false)
    {
        var sb = new StringBuilder();
        var count = 0;
        var appString = string.Empty;

        while (ex != null)
        {
            if (count > 0)
                appString += "  ";

            sb.AppendLine($"{appString}Message：{ex.Message}");
            sb.AppendLine($"{appString}Name：{ex.GetType().FullName}");
            sb.AppendLine($"{appString}Site：{ex.TargetSite?.Name}");
            sb.AppendLine($"{appString}Source：{ex.Source}");
            if (!isHideStackTrace && ex.StackTrace != null)
            {
                sb.AppendLine($"{appString}Stack：{ex.StackTrace}");
            }
            if (ex.InnerException != null)
            {
                sb.AppendLine($"{appString}Inner Exception：");
                count++;
            }

            ex = ex.InnerException;
        }

        return sb.ToString();
    }
}