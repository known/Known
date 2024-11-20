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
    private string MenuClass => Context.UserSetting.MenuTheme == "Dark" ? "kui-menu-dark" : "";

    /// <summary>
    /// 取得是否首次加载页面。
    /// </summary>
    protected bool IsLoaded { get; set; }

    /// <summary>
    /// 取得或设置用户设置信息。
    /// </summary>
    protected UserSettingInfo Setting { get; set; } = new();

    [CascadingParameter] private RouteData RouteData { get; set; }

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
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        await OnThemeColorAsync();
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
        if (!UIConfig.IgnoreRoutes.Contains(url) && !RouteData.PageType.IsAllowAnonymous())
        {
            if (Context.Current == null)
            {
                Navigation.GoErrorPage("403");
                return;
            }
        }
    }

    /// <summary>
    /// 模板呈现后异步操作方法，设置字体大小，语言，主题。
    /// </summary>
    /// <param name="firstRender">是否首次呈现。</param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            //JS不能在初始化中调用
            await JS.InitFilesAsync();
            if (Config.App.IsSize)
            {
                var size = await JS.GetCurrentSizeAsync();
                if (string.IsNullOrWhiteSpace(size))
                    size = Context.UserSetting.Size;
                if (string.IsNullOrWhiteSpace(size))
                    size = Config.App.DefaultSize;
                await JS.SetCurrentSizeAsync(size);
            }
            if (Config.App.IsLanguage)
            {
                var language = await JS.GetCurrentLanguageAsync();
                if (string.IsNullOrWhiteSpace(language))
                    language = Context.UserSetting.Language;
                Context.CurrentLanguage = language;
            }
        }
    }

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