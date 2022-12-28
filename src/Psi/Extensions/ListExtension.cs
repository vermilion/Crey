using Psi.Helper;

namespace Psi.Extensions;

public static class ListExtension
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

    private static void SortByDependenciesVisited<T>(T item, Func<T, IEnumerable<T>> getDependencies,
        ICollection<T> sorted, IDictionary<T, bool> visitDict)
    {
        if (visitDict.TryGetValue(item, out var visited))
        {
            if (visited)
            {

            }
        }
        else
        {
            visitDict[item] = true;
            var dependencies = getDependencies(item);
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    SortByDependenciesVisited(dependency, getDependencies, sorted, visitDict);
                }
            }
            visitDict[item] = false;
            sorted.Add(item);
        }
    }
}
