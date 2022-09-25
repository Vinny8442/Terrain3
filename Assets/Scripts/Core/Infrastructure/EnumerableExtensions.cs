using System;
using System.Collections.Generic;

namespace Core.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> target, Action<T> predicate)
        {
            foreach (T item in target)
            {
                predicate(item);
            }
        }
    }
}