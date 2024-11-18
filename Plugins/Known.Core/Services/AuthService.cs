namespace Known.Core.Services;

class AuthService
{
    //Account
    internal static async Task<UserInfo> GetUserAsync(IAdminService service, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        userName = userName.Split('-')[0];
        var user = Cache.GetUser(userName);
        if (user == null)
        {
            var db = Database.Create();
            user = await service.GetUserAsync(db, userName);
            Cache.SetUser(user);
        }
        return user;
    }
}