using Ultra.Extensions;
using Ultra.Infrastructure.Http.Interfaces;

namespace Ultra.Infrastructure.Http.Extensions
{
    public static class AuthApiClientExtensions
    {
        public static async Task<Dictionary<int, string>> GetUserNames(this IAuthApiClient client, IEnumerable<int> userIds)
        {
            var ids = userIds.Distinct().ToArray();
            return (await Task.WhenAll(ids.Select(async userId => new { Result = await client.GetUserName(userId), UserId = userId })))
                .Where(x => x.Result && x.Result.Object.IsNotNullOrEmpty())
                .ToDictionary(x => x.UserId, x => x.Result.Object);
        }
    }
}
