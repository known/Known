namespace Known.Internals;

/// <summary>
/// 多语言下拉框组件类。
/// </summary>
[NavItem]
public class NavLanguage : BaseComponent
{
    private ActionInfo current;

    /// <summary>
    /// 取得或设置下拉框图标，默认为translation。
    /// </summary>
    [Parameter] public string Icon { get; set; } = "translation";

    /// <summary>
    /// 异步初始化多语言下拉框组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        current = Language.GetLanguage(Context.CurrentLanguage);
    }

    /// <summary>
    /// 呈现多语言下拉框组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Dropdown(new DropdownModel
        {
            Icon = Icon,
            Text = current?.Icon,
            Items = Language.Items.Where(i => i.Visible).ToList(),
            OnItemClick = OnLanguageChangedAsync
        });
    }

    private async Task OnLanguageChangedAsync(ActionInfo info)
    {
        current = info;
        Context.CurrentLanguage = current.Id;
        if (CurrentUser != null)
        {
            Context.UserSetting.Language = current.Id;
            await Data.SaveUserSettingAsync(Context.UserSetting);
        }
        await JS.SetCurrentLanguageAsync(current.Id);
        Navigation.Refresh(true);
    }
}