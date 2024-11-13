using Microsoft.AspNetCore.Identity;

namespace Known.Core.Auths;

class UserStore(Database database, IPlatformService platform) : IUserStore<UserInfo>
{
    public Task<IdentityResult> CreateAsync(UserInfo user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(UserInfo user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public void Dispose()
    {
        //throw new NotImplementedException();
    }

    public Task<UserInfo> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        // TODO: get from cache
        return platform.GetUserByIdAsync(database, userId);
    }

    public Task<UserInfo> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserInfo());
    }

    public Task<string> GetNormalizedUserNameAsync(UserInfo user, CancellationToken cancellationToken)
    {
       return Task.FromResult(user.UserName.ToUpper());
    }

    public Task<string> GetUserIdAsync(UserInfo user, CancellationToken cancellationToken)
    {
       return Task.FromResult(user.Id);
    }

    public Task<string> GetUserNameAsync(UserInfo user, CancellationToken cancellationToken)
    {
       return Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(UserInfo user, string normalizedName, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(UserInfo user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> UpdateAsync(UserInfo user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }
}