namespace Known;

public interface IAuthStateProvider
{
    Task<UserInfo> GetUserAsync();
    Task UpdateUserAsync(UserInfo user);
}