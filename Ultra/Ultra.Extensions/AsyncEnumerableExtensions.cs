using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async IAsyncEnumerable<T> WhereAsync<T>(this IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            foreach (var item in source)
            {
                if (await predicate(item))
                {
                    yield return item;
                }
            }
        }

        public static async IAsyncEnumerable<E> SelectAsync<T, E>(this IEnumerable<T> source, Func<T, Task<E>> selector)
        {
            foreach (var item in source)
            {
                yield return await selector(item);
            }
        }

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            var list = new List<T>();
            await foreach (var item in source)
            {
                list.Add(item);
            }
            return list;
        }

        public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source)
        {
            return (await source.ToListAsync()).ToArray();
        }
    }
}
