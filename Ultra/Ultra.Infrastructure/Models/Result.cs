using System.Net;
using Ultra.Infrastructure.Exceptions;

namespace Ultra.Infrastructure.Models
{
    public class Result<T> : Result
    {
        public T Object { get; set; }

        public new Task<Result<T>> AsTask() => Task.FromResult(this);
        public T GetObjectOrThrow() =>
            IsSuccess 
                ? Object 
                : throw new ApiException(Error, StatusCode, Exception);

        public T GetObjectOrDefault(T _default = default) =>
            IsSuccess
                ? Object
                : _default;

        public static implicit operator T(Result<T> result) => result.Object;
        public static implicit operator Result<T>(T obj) => Success(obj);
        public static implicit operator Task<Result<T>>(Result<T> result) => result.AsTask();

        public override string ToString()
        {
            return IsSuccess
                ? Object.ToString()
                : $"{StatusCode}: {Error}";
        }
    }

    public class Result
    {
        private const string NotFoundMessage = "Не найдена запись";

        public bool IsSuccess => 200 <= (int)StatusCode && (int)StatusCode < 300;
        public HttpStatusCode StatusCode { get; set; }
        public string? Error { get; set; }
        public Exception? Exception { get; set; }


        public Task<Result> AsTask() => Task.FromResult(this);
        public Result<T> AsResult<T>() => new Result<T>
        {
            StatusCode = this.StatusCode,
            Error = this.Error,
            Exception = this.Exception,
        };


        public static Result Success() => new() { StatusCode = HttpStatusCode.OK };
        public static Result Failed() => new() { StatusCode = HttpStatusCode.BadRequest };
        public static Result Failed(string error) => new() { Error = error, StatusCode = HttpStatusCode.BadRequest };
        public static Result NotFound(string? error = null) => new() { Error = error ?? NotFoundMessage, StatusCode = HttpStatusCode.NotFound };
        public static Result Failed(Exception ex) => new() { Exception = ex, StatusCode = HttpStatusCode.BadRequest };
        public static Result Failed(Exception ex, string error, HttpStatusCode code) => new() { Error = error, Exception = ex, StatusCode = code };

        public static Result<T> Success<T>(T obj) => new() { Object = obj, StatusCode = HttpStatusCode.OK };
        public static Result<T> Failed<T>() => new() { StatusCode = HttpStatusCode.BadRequest };
        public static Result<T> Failed<T>(string error) => new() { Error = error, StatusCode = HttpStatusCode.BadRequest };
        public static Result<T> NotFound<T>(string? error = null) => new() { Error = error ?? NotFoundMessage, StatusCode = HttpStatusCode.NotFound };
        public static Result<T> Failed<T>(Exception ex) => new() { Exception = ex, StatusCode = HttpStatusCode.BadRequest };
        public static Result<T> Failed<T>(Exception ex, string error, HttpStatusCode code) => new() { Error = error, Exception = ex, StatusCode = code };


        public static implicit operator bool(Result result) => result.IsSuccess;
        public static implicit operator Task<Result>(Result result) => result.AsTask();

        public override string ToString()
        {
            return IsSuccess
                ? StatusCode.ToString()
                : $"{StatusCode}: {Error}";
        }
    }
}
