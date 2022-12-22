using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Spear.Core.Helper;

namespace Spear.Core.Extensions
{
    /// <summary> 列表扩展 </summary>
    public static class ListExtension
    {
        /// <summary> 判断列表为空 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return true;
            var array = source.ToArray();
            return !array.Any() || array.All(t => t == null || string.IsNullOrWhiteSpace(t.ToString()));
        }

        /// <summary> 随机排序 </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
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
}
