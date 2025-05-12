namespace Known.Internals;

/// <summary>
/// 多语言下拉框组件类。
/// </summary>
[NavPlugin("多语言", "translation", Category = "组件", Sort = 4)]
public class NavLanguage : BaseNav
{
    private LanguageSettingInfo current;

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
            Text = current?.Icon,
            Items = items,
            OnItemClick = OnLanguageChangedAsync
        });
    }

    private async Task OnLanguageChangedAsync(ActionInfo info)
    {
        current = Language.Settings.FirstOrDefault(l => l.Id == info.Id);
        Context.CurrentLanguage = current.Id;
        if (CurrentUser != null)
        {
            Context.UserSetting.Language = current.Id;
            await Admin.SaveUserSettingAsync(Context.UserSetting);
        }
        await JS.SetCurrentLanguageAsync(current.Id);
        Navigation.Refresh();
    }
}