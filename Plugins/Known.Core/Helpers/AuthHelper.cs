namespace Known.Core.Helpers;

class AuthHelper
{
    //Account
    internal static async Task<UserInfo> GetUserAsync(IAdminService platform, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        userName = userName.Split('-')[0];
        var user = Cache.GetUser(userName);
        if (user == null)
        {
            user = await platform.GetUserAsync(userName);
            Cache.SetUser(user);
        }
        return user;
    }

    internal static async Task<UserInfo> GetUserByIdAsync(IAdminService platform, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        var user = Cache.GetUser(userId);
        if (user == null)
        {
            user = await platform.GetUserByIdAsync(userId);
            Cache.SetUser(user);
        }
        return user;
    }
}