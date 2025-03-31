namespace Known.Internals;

/// <summary>
/// 用户信息组件类。
/// </summary>
public partial class UserProfileInfo
{
    private UserInfo user;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        user = CurrentUser;
    }

    private async Task OnFileChangedAsync(InputFileChangeEventArgs e)
    {
        var file = await e.File.CreateFileAsync();
        var info = new AvatarInfo { UserId = CurrentUser?.Id, File = file };
        var result = await Admin?.UpdateAvatarAsync(info);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
            return;
        }

        var user = CurrentUser;
        user.AvatarUrl = result.DataAs<string>();
        App?.SignInAsync(user);
        Navigation.Refresh(true);
    }
}