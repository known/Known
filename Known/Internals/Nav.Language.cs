namespace Known.Internals;

/// <summary>
/// 多语言下拉框组件类。
/// </summary>
[NavPlugin(Language.NavLanguage, "translation", Category = Language.Component, Sort = 5)]
public class NavLanguage : BaseNav
{
    private LanguageSettingInfo current;

    /// <summary>
    /// 取得或设置是否显示名称。
    /// </summary>
    [Parameter] public bool ShowName { get; set; }

    /// <summary>
    /// 取得图标。
    /// </summary>
    protected override string Icon => "translation";

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        current = Language.GetLanguage(Context.CurrentLanguage);
    }

    /// <summary>
    /// 呈现组件内容。
    /// </summary>
    /// <param name="builder">呈现建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var items = Language.Settings.Where(l => l.Enabled).Select(l => new ActionInfo { Id = l.Id, Name = l.Name, Icon = l.Icon }).ToList();
        builder.Dropdown(new DropdownModel
        {
            Icon = Icon,
            Text = ShowName ? current?.Name : current?.Icon,
            Items = items,
            OnItemClick = OnLanguageChangedAsync
        });
    }

    private async Task OnLanguageChangedAsync(ActionInfo info)
    {
        current = Language.Settings.FirstOrDefault(l => l.Id == info.Id);
        // 设置当前语言（Culture）
        var culture = new CultureInfo(current.Code);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        // 设置上下文
        Context.CurrentLanguage = current.Code;
        Context.Local.Language = current.Code;
        await SetLocalInfoAsync(Context.Local);
        Navigation.Refresh();
    }
}