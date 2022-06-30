/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Known.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Known.Razor;

public abstract class BaseComponent : ComponentBase, IDisposable
{
    [Inject] protected PlatformService Platform { get; set; }
    [Inject] protected UIService UI { get; set; }
    [Inject] protected NavigationManager Navigation { get; set; }
    [Inject] protected HttpClient Http { get; set; }
    [Inject] protected IJSRuntime JSRuntime { get; set; }
    [Inject] protected AuthStateProvider AuthProvider { get; set; }

    [CascadingParameter] protected AppContext AppContext { get; set; }
    [CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; }

    protected bool IsAuthenticated
    {
        get { return AuthState.Result.User.Identity.IsAuthenticated; }
    }

    private UserInfo currentUser;
    protected UserInfo CurrentUser
    {
        get
        {
            if (currentUser != null)
                return currentUser;

            var authState = AuthState.Result;
            if (authState.User.Identity.IsAuthenticated)
            {
                var user = authState.User.Identity.Name;
                //currentUser = UserHelper.GetUser(out _);
                currentUser = new UserInfo { UserName = user };
            }
            return currentUser;
        }
        set
        {
            currentUser = value;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected static RenderFragment BuildTree(Action<RenderTreeBuilder> action)
    {
        return delegate (RenderTreeBuilder builder) { action(builder); };
    }

    protected static RenderFragment<object> BuildTree(Action<RenderTreeBuilder, object> action)
    {
        return (object row) => delegate (RenderTreeBuilder builder) { action(builder, row); };
    }

    protected static RenderFragment<T> BuildTree<T>(Action<RenderTreeBuilder, T> action)
    {
        return (T row) => delegate (RenderTreeBuilder builder) { action(builder, row); };
    }

    protected EventCallback Callback(EventCallback action)
    {
        return EventCallback.Factory.Create(this, action);
    }

    protected EventCallback Callback(Action<object> action)
    {
        return EventCallback.Factory.Create(this, action);
    }

    protected EventCallback<T> Callback<T>(Action<T> action)
    {
        return EventCallback.Factory.Create(this, action);
    }

    protected static string FormatDate(object date, string format = "yyyy-MM-dd")
    {
        if (date == null)
            return string.Empty;

        return ((DateTime)date).ToString(format);
    }
}
