using System.Text;
using System.Net.Mime;
using Ultra.Extensions;
using Ultra.Infrastructure.Models;

namespace Ultra.Infrastructure.Http.Abstract
{
    public class HttpClientBase
    {
        private readonly IHttpClientFactory _factory;
        private readonly string _baseUrl;

        public HttpClientBase(IHttpClientFactory factory, string baseUrl)
        {
            _factory = factory;
            _baseUrl = baseUrl;
        }

        protected HttpClient GetClient()
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_baseUrl);
            return client;
        }

        public virtual HttpResponseMessage Send(Func<HttpRequestMessage> requestGenerator, CancellationToken cancellationToken)
        {
            var client = GetClient();
            return client.Send(requestGenerator(), cancellationToken);
        }

        public virtual Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> requestGenerator, CancellationToken cancellationToken)
        {
            var client = GetClient();
            return client.SendAsync(requestGenerator());
        }

        private static HttpContent? GetHttpContent(object? reqObj = null)
        {
            HttpContent? content = null;
            if (reqObj != null)
            {
                if (reqObj is HttpContent reqHttpContent)
                {
                    content = reqHttpContent;
                }
                else
                {
                    content = new StringContent(reqObj.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json);
                }
            }

            return content;
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(() => new HttpRequestMessage
            {
                RequestUri = new Uri(url, UriKind.Relative),
                Method = HttpMethod.Get,
            }, cancellationToken);

            return (await response.Content.ReadAsStringAsync(cancellationToken)).FromJson<T>()!;
        }

        public async Task<T> PostAsync<T>(string url, object? reqObj = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(() => new HttpRequestMessage
            {
                Content = GetHttpContent(reqObj),
                RequestUri = new Uri(url, UriKind.Relative),
                Method = HttpMethod.Post,
            }, cancellationToken);

            return (await response.Content.ReadAsStringAsync(cancellationToken)).FromJson<T>()!;
        }

        public async Task<T> PutAsync<T>(string url, object? reqObj = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(() => new HttpRequestMessage
            {
                Content = GetHttpContent(reqObj),
                RequestUri = new Uri(url, UriKind.Relative),
                Method = HttpMethod.Put,
            }, cancellationToken);

            return (await response.Content.ReadAsStringAsync(cancellationToken)).FromJson<T>()!;
        }

        public async Task<T> DeleteAsync<T>(string url, object? reqObj = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(() => new HttpRequestMessage
            {
                Content = GetHttpContent(reqObj),
                RequestUri = new Uri(url, UriKind.Relative),
                Method = HttpMethod.Delete,
            }, cancellationToken);

            return (await response.Content.ReadAsStringAsync(cancellationToken)).FromJson<T>()!;
        }

        public Task<Result<T>> GetResultAsync<T>(string url, CancellationToken cancellationToken = default)
            => GetAsync<Result<T>>(url, cancellationToken);

        public Task<Result<T>> PostResultAsync<T>(string url, object? reqObj = null, CancellationToken cancellationToken = default)
            => PostAsync<Result<T>>(url, reqObj, cancellationToken);

        public Task<Result<T>> PutResultAsync<T>(string url, object? reqObj = null, CancellationToken cancellationToken = default)
            => PutAsync<Result<T>>(url, reqObj, cancellationToken);

        public Task<Result<T>> DeleteResultAsync<T>(string url, object? reqObj = null, CancellationToken cancellationToken = default)
            => DeleteAsync<Result<T>>(url, reqObj, cancellationToken);

        public Task<Result> GetResultAsync(string url, CancellationToken cancellationToken = default)
            => GetAsync<Result>(url, cancellationToken);

        public Task<Result> PostResultAsync(string url, object? reqObj = null, CancellationToken cancellationToken = default)
            => PostAsync<Result>(url, reqObj, cancellationToken);

        public Task<Result> PutResultAsync(string url, object? reqObj = null, CancellationToken cancellationToken = default)
            => PutAsync<Result>(url, reqObj, cancellationToken);

        public Task<Result> DeleteResultAsync(string url, object? reqObj = null, CancellationToken cancellationToken = default)
            => DeleteAsync<Result>(url, reqObj, cancellationToken);
    }
}
