namespace Known.Internals;

/// <summary>
/// 多语言下拉框组件类。
/// </summary>
[NavPlugin(Language.NavLanguage, "translation", Category = Language.Component, Sort = 4)]
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
        Context.CurrentLanguage = current.Code;
        if (CurrentUser != null)
        {
            Context.UserSetting.Language = current.Code;
            await Admin.SaveUserSettingAsync(Context.UserSetting);
        }
        Context.Local.Language = current.Code;
        await JS.SetLocalInfoAsync(Context.Local, isLanguage: true);
        Navigation.Refresh();
    }
}