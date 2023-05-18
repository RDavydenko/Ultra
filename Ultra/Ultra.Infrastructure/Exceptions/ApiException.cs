using System.Net;

namespace Ultra.Infrastructure.Exceptions
{
    // TODO: фильтр для его обработки
    public class ApiException : Exception
    {
        public HttpStatusCode? StatusCode { get; }

        public ApiException(string? message = null, HttpStatusCode? statusCode = null, Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
