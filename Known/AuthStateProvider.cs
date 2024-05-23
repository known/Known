namespace Known;

public interface IAuthStateProvider
{
    Task<UserInfo> GetUserAsync();
    Task SetUserAsync(UserInfo user);
}