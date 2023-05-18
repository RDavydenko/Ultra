using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string? s) => string.IsNullOrEmpty(s);
        public static bool IsNullOrWhiteSpace(this string? s) => string.IsNullOrWhiteSpace(s);

        public static bool IsNotNullOrEmpty(this string? s) => !s.IsNullOrEmpty();
        public static bool IsNotNullOrWhiteSpace(this string? s) => !s.IsNullOrWhiteSpace();
    }
}
