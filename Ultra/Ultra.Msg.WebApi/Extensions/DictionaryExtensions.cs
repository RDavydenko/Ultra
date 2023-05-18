using Ultra.Msg.WebApi.Models.Constants;

namespace Ultra.Msg.WebApi.Extensions
{
    public static class DictionaryExtensions
    {
        public static string GetUserNameOrDefault(
            this Dictionary<int, string> userNames, 
            int userId,
            string _default = UserConstants.UndefinedUserName)
        {
            return userNames.GetValueOrDefault(userId, _default);
        }
    }
}
