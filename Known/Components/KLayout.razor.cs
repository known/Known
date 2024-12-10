using AntDesign;

namespace Known.Components;

/// <summary>
/// 后台布局模板组件类。
/// </summary>
partial class KLayout
{
    private string spinTip = "";
    private bool showSpin = false;
    private bool showSetting = false;
    private bool collapsed = false;

    private AntMenu menu;
    private ReloadContainer reload;

    private string WrapperClass => CssBuilder.Default("kui-wrapper").AddClass(Setting.Size).BuildClass();
    private string TabsClass => CssBuilder.Default("kui-nav-tabs").AddClass("is-top", Setting.IsTopTab).BuildClass();
    private string HeaderClass => CssBuilder.Default("kui-header")
                                            .AddClass("kui-menu-dark", Setting.MenuTheme == "Dark")
                                            .BuildClass();
    private string MenuClass => CssBuilder.Default()
                                          .AddClass("kui-menu-dark", Setting.MenuTheme == "Dark")
                                          .AddClass("kui-menu-float", Setting.LayoutMode == LayoutMode.Float.ToString())
                                          .BuildClass();

    [Inject] private ReuseTabsService Service { get; set; }

    /// <summary>
    /// 取得或设置页面是否加载完成。
    /// </summary>
    protected bool IsLoaded { get; set; } = true;

    /// <summary>
    /// 取得或设置页面是否进行URL鉴权。
    /// </summary>
    protected bool IsUrlAuth { get; set; }

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
    /// 重新加载当前页面，如果是多标签，则刷新当前标签页。
    /// </summary>
    public override void ReloadPage()
    {
        if (Setting.MultiTab)
            Service.ReloadPage();
        else
            reload?.Reload();
    }

    /// <summary>
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        Context.Url = Navigation.GetPageUrl();
        Context.SetCurrentMenu(RouteData);
        CheckUrlAuthentication();
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
            }
            if (menu != null)
                await menu.SetItemsAsync(UserMenus);
        }
        catch (Exception ex)
        {
            await OnErrorAsync(ex);
        }
    }

    internal override void ToggleSide(bool collapsed)
    {
        this.collapsed = collapsed;
        StateChanged();
    }

    private void CheckUrlAuthentication()
    {
        if (!IsUrlAuth)
            return;

        if (!UIConfig.IgnoreRoutes.Contains(Context.Url) && !RouteData.PageType.IsAllowAnonymous())
        {
            if (Context.Current == null)
            {
                Navigation.GoErrorPage("403");
                return;
            }
        }
    }

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