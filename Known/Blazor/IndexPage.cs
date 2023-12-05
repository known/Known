﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Known.Blazor;

public class IndexPage : BaseComponent
{
    private const string KeyUIType = "Known_UIType";
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; }
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; }

    protected bool IsLogin { get; private set; }
    protected string UIType { get; private set; }

    public void SetUIType(string type)
    {
        UIType = type;
        JS.SetLocalStorage(KeyUIType, type);
    }

    protected override async Task OnInitializedAsync()
    {
        IsLoaded = false;
        Context.Install = await Platform.System.GetInstallAsync();
        Context.CurrentUser = await GetCurrentUserAsync();
        IsLogin = Context.CurrentUser != null;
        UIType = await JS.GetLocalStorage<string>(KeyUIType);
        IsLoaded = true;
    }

    protected virtual Task<UserInfo> GetThirdUserAsync()
    {
        UserInfo user = null;
        return Task.FromResult(user);
    }

    protected virtual async Task<UserInfo> GetCurrentUserAsync()
    {
        if (AuthState == null)
            return null;

        var state = await AuthState;
        if (state != null && state.User != null && state.User.Identity != null && state.User.Identity.IsAuthenticated)
        {
            if (AuthProvider is IAuthStateProvider provider)
                return await provider.GetUserAsync();
        }

        return await GetThirdUserAsync();
    }

    protected virtual async Task SetCurrentUserAsync(UserInfo user)
    {
        if (AuthProvider is IAuthStateProvider provider)
        {
            await provider.UpdateUserAsync(user);
        }
    }

    protected void OnInstall(InstallInfo install)
    {
        Context.Install = install;
        StateChanged();
    }

    protected async void OnLogin(UserInfo user)
    {
        Context.CurrentUser = user;
        IsLogin = Context.CurrentUser != null;
        await SetCurrentUserAsync(user);
        StateChanged();
    }

    protected async void OnLogout()
    {
        IsLogin = false;
        await SetCurrentUserAsync(null);
        StateChanged();
    }
}