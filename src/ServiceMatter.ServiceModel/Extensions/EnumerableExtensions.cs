using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ServiceMatter.ServiceModel.Extensions
{
    public static class EnumerableExtensions
    {
        public static void DoForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            Debug.Assert(action != null);

            if (enumeration == null) return;

            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
