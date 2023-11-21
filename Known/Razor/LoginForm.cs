using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Razor;

public class LoginForm : BaseComponent
{
    private readonly string KeyLoginInfo = "LoginInfo";
    protected LoginFormInfo Model = new();

    [Parameter] public Action<UserInfo> OnLogin { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var info = await JS.GetLocalStorage<LoginInfo>(KeyLoginInfo);
            if (info != null)
            {
                Model.UserName = info.UserName;
                Model.Remember = info.Remember;
                StateChanged();
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected async Task OnUserLogin(EditContext context)
    {
        if (!Model.Remember)
        {
            JS.SetLocalStorage(KeyLoginInfo, null);
        }
        else
        {
            JS.SetLocalStorage(KeyLoginInfo, new LoginInfo
            {
                UserName = Model.UserName,
                Remember = Model.Remember
            });
        }

        var result = await Platform.Auth.SignInAsync(Model);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
        }
        else
        {
            var user = result.DataAs<UserInfo>();
            OnLogin?.Invoke(user);
        }
    }

    class LoginInfo
    {
        public string UserName { get; set; }
        public bool Remember { get; set; }
    }
}