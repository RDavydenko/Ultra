using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Ultra.Infrastructure.Models.Auth.Configuration;
using Ultra.Extensions;
using Ultra.Infrastructure.Models.Auth;

namespace Ultra.Infrastructure.Http.Abstract
{
    public class AuthorizedHttpClientBase : HttpClientBase
    {
        private readonly IHttpClientFactory _factory;
        private readonly IOptionsMonitor<AuthByPasswordOptions> _options;
        private static string _access_token;

        public AuthorizedHttpClientBase(
            IHttpClientFactory factory,
            IOptionsMonitor<AuthByPasswordOptions> options,
            string baseUrl)
            : base(factory, baseUrl)
        {
            _factory = factory;
            _options = options;
            _options.OnChange((v, s) => Authorize());
        }

        private async Task Authorize()
        {
            var value = _options.CurrentValue;

            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(value.IdentityUrl);

            using var response = await client.PostAsync(
                "connect/token",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", value.GrantType),
                    new KeyValuePair<string, string>("client_id", value.ClientId),
                    new KeyValuePair<string, string>("username", value.UserName),
                    new KeyValuePair<string, string>("password", value.Password),
                }));
            var result = (await response.Content.ReadAsStringAsync()).FromJson<OAuthTokenConnectModel>();
            if (result != null)
            {
                _access_token = result.AccessToken;
            }
        }

        public override HttpResponseMessage Send(Func<HttpRequestMessage> requestGenerator, CancellationToken cancellationToken)
        {
            var result = base.Send(() =>
            {
                var request = requestGenerator();
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_access_token}");
                return request;
            }, cancellationToken);
            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Authorize().GetAwaiter().GetResult();
            }
            else
            {
                return result;
            }

            return base.Send(() =>
            {
                var request = requestGenerator();
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_access_token}");
                return request;
            }, cancellationToken);
        }

        public override async Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> requestGenerator, CancellationToken cancellationToken)
        {
            var result = await base.SendAsync(() =>
            {
                var request = requestGenerator();
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_access_token}");
                return request;
            }, cancellationToken);
            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await Authorize();
            }
            else
            {
                return result;
            }

            return await base.SendAsync(() =>
            {
                var request = requestGenerator();
                request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_access_token}");
                return request;
            }, cancellationToken);
        }
    }
}
