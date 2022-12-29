using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using Psi.Extensions;

namespace Psi.Exceptions;

public static class ErrorCodesExtension
{
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IDictionary<int, string>> ErrorCodesCache = new();

    private static readonly Type ErrorType = typeof(ErrorCodes);

    public static string Message<T>(this int code) where T : ErrorCodes
    {
        var codes = Codes<T>();
        return codes.TryGetValue(code, out var message) ? message : ErrorCodes.SystemError.Message<ErrorCodes>();
    }

    public static PsiException CodeException<T>(this int code, string message = null) where T : ErrorCodes
    {
        message = string.IsNullOrWhiteSpace(message) ? code.Message<T>() : message;
        return new PsiException(message, code);
    }

    public static PsiException CodeException(this int code, string message = null)
    {
        return code.CodeException<ErrorCodes>(message);
    }

    public static IDictionary<int, string> Codes<T>() where T : ErrorCodes
    {
        return typeof(T).Codes();
    }

    public static IDictionary<int, string> Codes(this Type type)
    {
        if (type != ErrorType && !type.IsSubclassOf(ErrorType))
            return new Dictionary<int, string>();

        var key = type.TypeHandle;
        if (ErrorCodesCache.ContainsKey(key) && ErrorCodesCache.TryGetValue(type.TypeHandle, out var codes))
            return codes;

        codes = new Dictionary<int, string>();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        foreach (var field in fields)
        {
            var message = field.GetCustomAttribute<DescriptionAttribute>()?.Description ?? field.Name;
            codes.Add(field.GetRawConstantValue().CastTo<int>(), message);
        }

        while (type != null && type != ErrorType)
        {
            type = type.BaseType;
            foreach (var t in type.Codes())
            {
                codes.Add(t.Key, t.Value);
            }
        }

        var orderCodes = codes.OrderBy(t => t.Key).ToDictionary(k => k.Key, v => v.Value);
        ErrorCodesCache.TryAdd(key, orderCodes);
        return orderCodes;
    }
}
