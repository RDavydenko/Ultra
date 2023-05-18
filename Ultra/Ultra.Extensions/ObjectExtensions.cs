using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ultra.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static Task<T> AsTask<T>(this T obj) => 
            Task.FromResult(obj);
    }
}
