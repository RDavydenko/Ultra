using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ultra.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static async Task ForEach<T>(this IEnumerable<T> source, Func<T, Task> func)
        {
            foreach (var item in source)
            {
                await func(item);
            }
        }

        public static IEnumerable<T> SelectNotNull<T, E>(this IEnumerable<E> source, Func<E, T?> selector)
            where T : struct
            => source.Select(selector).Where(x => x.HasValue).Select(x => x!.Value);
    }
}
