using System;

namespace Ultra.Extensions
{
    public static class ByteExtensions
    {
        public static string ToBase64(this byte[] bytes) => Convert.ToBase64String(bytes);
        public static byte[] FromBase64(this string str) => Convert.FromBase64String(str);
    }
}
