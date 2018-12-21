using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ServiceMatter.ServiceModel.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> DoForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            Debug.Assert(enumerable != null);
            Debug.Assert(action != null);

            foreach (var item in enumerable)
            {
                action(item);
                yield return item;
            }
        }
    }
}
