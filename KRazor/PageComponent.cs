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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

[Authorize]
public abstract class PageComponent : BaseComponent
{
    protected virtual bool CheckLogin { get; } = true;
    protected bool IsCheckKey { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        IsCheckKey = CheckKey(out _);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!IsCheckKey)
            BuildAuthorize(builder);
        else
            BuildPage(builder);
    }

    protected virtual void BuildAuthorize(RenderTreeBuilder builder) { }
    protected virtual void BuildPage(RenderTreeBuilder builder) { }

    protected virtual bool CheckKey(out string message)
    {
        message = string.Empty;
        return true;
    }

    protected void NavigateToLogin()
    {
        Navigation.NavigateTo("/login");
    }
}
