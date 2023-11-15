using Known.Entities;
using Known.Extensions;

namespace Known.Razor;

public class SysUserProfile : BasePage
{
    protected SysUser User;
    protected PwdFormInfo PwdModel = new();
    protected bool IsEdit = false;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        User = await Platform.User.GetUserByIdAsync(CurrentUser.Id);
    }

    protected void OnEdit(bool edit) => IsEdit = edit;

    protected async Task OnSaveUserInfo()
    {
        var result = await Platform.User.UpdateUserAsync(User);
        UI.Result(result, () => IsEdit = false);
    }

    protected async Task OnSavePassword()
    {
        var result = await Platform.User.UpdatePasswordAsync(PwdModel);
        UI.Result(result);
    }
}