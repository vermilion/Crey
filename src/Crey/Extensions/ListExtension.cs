using Crey.Helper;

namespace Crey.Extensions;

internal static class ListExtension
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        if (source == null)
            return true;

        var array = source.ToArray();

        return !array.Any() || array.All(t => t == null || string.IsNullOrWhiteSpace(t.ToString()));
    }

    public static IEnumerable<T> RandomSort<T>(this IEnumerable<T> array)
    {
        return array.OrderBy(t => RandomHelper.Random().Next());
    }
}
