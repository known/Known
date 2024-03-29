﻿using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class LoginPage : BaseComponent
{
    protected LoginFormInfo Model = new();

    [Parameter] public Func<UserInfo, Task> OnLogin { get; set; }

    protected override async Task OnInitAsync()
    {
        var info = await JS.GetLoginInfoAsync<LoginInfo>();
        if (info != null)
        {
            Model.UserName = info.UserName;
            Model.PhoneNo = info.PhoneNo;
            Model.Remember = info.Remember;
            Model.Station = info.Station;
            Model.TabKey = info.TabKey;
        }
    }

    protected virtual void OnSigning() { }

    protected async Task OnUserLogin()
    {
        if (!Model.Remember)
        {
            await JS.SetLoginInfoAsync(null);
        }
        else
        {
            await JS.SetLoginInfoAsync(new LoginInfo
            {
                UserName = Model.UserName,
                PhoneNo = Model.PhoneNo,
                Remember = Model.Remember,
                Station = Model.Station,
                TabKey = Model.TabKey
            });
        }

        OnSigning();
        Model.IPAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var result = await Platform.Auth.SignInAsync(Model);
        if (!result.IsValid)
        {
            UI.Error(result.Message);
        }
        else
        {
            var user = result.DataAs<UserInfo>();
            await OnLogin?.Invoke(user);
        }
    }

    class LoginInfo
    {
        public string UserName { get; set; }
        public string PhoneNo { get; set; }
        public bool Remember { get; set; }
        public string Station { get; set; }
        public string TabKey { get; set; }
    }
}