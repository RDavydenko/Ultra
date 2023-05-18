using Microsoft.Extensions.Logging;
using System;

namespace Ultra.Extensions
{ 
    public static class ILoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception ex)
        {
            logger.LogError(ex, null);
        }
    }
}
