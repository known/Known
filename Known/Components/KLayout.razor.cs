namespace Known.Components;

/// <summary>
/// 后台布局模板组件类。
/// </summary>
partial class KLayout
{
    private string spinTip = "";
    private bool showSpin = false;
    private bool collapsed = false;
    private bool showSetting = false;
    private string HeaderClass => Setting.MenuTheme == "Dark" ? "kui-header kui-menu-dark" : "kui-header";
    private string MenuClass => Setting.MenuTheme == "Dark" ? "kui-menu-dark" : "";
    private AntMenu menu;

    /// <summary>
    /// 取得是否首次加载页面。
    /// </summary>
    protected bool IsLoaded { get; set; } = true;

    /// <summary>
    /// 取得或设置用户设置信息。
    /// </summary>
    protected UserSettingInfo Setting { get; set; } = new();

    /// <summary>
    /// 取得或设置路由数据对象。
    /// </summary>
    [CascadingParameter] protected RouteData RouteData { get; set; }

    /// <summary>
    /// 取得当前用户权限菜单列表。
    /// </summary>
    [Parameter] public List<MenuInfo> UserMenus { get; set; }

    /// <summary>
    /// 取得或设置子组件内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// 异步显示快速旋转加载提示。
    /// </summary>
    /// <param name="text">提示文本。</param>
    /// <param name="action">耗时操作委托。</param>
    /// <returns></returns>
    public override async Task ShowSpinAsync(string text, Func<Task> action)
    {
        if (action == null)
            return;

        showSpin = true;
        spinTip = text;
        await StateChangedAsync();
        await Task.Run(async () =>
        {
            try
            {
                await action?.Invoke();
            }
            catch (Exception ex)
            {
                await OnErrorAsync(ex);
            }
            showSpin = false;
            await StateChangedAsync();
        });
    }

    /// <summary>
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        var url = Navigation.GetPageUrl();
        var pageRoute = url.StartsWith("/page/") ? url.Substring(6) : "";
        Context.Url = url;
        Context.SetCurrentMenu(RouteData, pageRoute);
    }

    /// <summary>
    /// 模板呈现后异步操作方法，设置字体大小，语言，主题。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        try
        {
            if (firstRender)
            {
                await JS.InitFilesAsync();
                await OnThemeColorAsync();
                if (Config.App.IsSize)
                {
                    var size = await JS.GetCurrentSizeAsync();
                    if (string.IsNullOrWhiteSpace(size))
                        size = Setting.Size;
                    if (string.IsNullOrWhiteSpace(size))
                        size = Config.App.DefaultSize;
                    await JS.SetCurrentSizeAsync(size);
                }
                if (Config.App.IsLanguage)
                {
                    var language = await JS.GetCurrentLanguageAsync();
                    if (string.IsNullOrWhiteSpace(language))
                        language = Setting.Language;
                    Context.CurrentLanguage = language;
                }
                await OnRenderAfterAsync(firstRender);
            }
            if (menu != null)
                await menu.SetItemsAsync(UserMenus);
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }

    /// <summary>
    /// 模板呈现后异步操作虚方法。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected virtual Task OnRenderAfterAsync(bool firstRender) => Task.CompletedTask;

    /// <summary>
    /// 异步获取用户设置信息。
    /// </summary>
    /// <returns>用户设置信息。</returns>
    protected virtual Task<UserSettingInfo> GetUserSettingAsync() => Task.FromResult(new UserSettingInfo());

    private void OnToggle() => collapsed = !collapsed;

    private async Task OnThemeColorAsync()
    {
        var theme = Setting.ThemeColor;
        var href = $"_content/Known/css/theme/{theme}.css";
        await JS.SetStyleSheetAsync("/theme/", href);
    }

    private async Task OnSaveSetting()
    {
        var result = await Platform.SaveUserSettingAsync(Setting);
        if (result.IsValid)
        {
            Context.UserSetting = Setting;
            await OnThemeColorAsync();
        }
    }

    private Task OnResetSetting()
    {
        Setting = new();
        return OnSaveSetting();
    }

    private void OnMenuClick(string id)
    {
        switch (id)
        {
            case "logout":
                UI?.Confirm(Language["Tip.Exits"], SignOutAsync);
                break;
            case "setting":
                showSetting = true;
                StateHasChanged();
                break;
        }
    }
}