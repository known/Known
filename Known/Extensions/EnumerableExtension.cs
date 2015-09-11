using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class EnumerableExtension
    {
        public static void ForEach<T>(this ICollection collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action((T)item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in collection)
            {
                action(item, i++);
            }
        }
    }
}
