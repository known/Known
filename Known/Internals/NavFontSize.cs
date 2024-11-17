namespace Known.Internals;

/// <summary>
/// 系统字体大小下拉框组件类。
/// </summary>
[NavItem]
class NavFontSize : BaseComponent
{
    /// <summary>
    /// 取得或设置下拉框图标，默认为font-size。
    /// </summary>
    [Parameter] public string Icon { get; set; } = "font-size";

    /// <summary>
    /// 取得或设置系统字体大小下拉框项目列表。
    /// </summary>
    [Parameter] public List<ActionInfo> Items { get; set; }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        UIConfig.Sizes.ForEach(s => s.Name = Language[$"Nav.Size{s.Id}"]);
    }

    /// <summary>
    /// 呈现系统字体下拉框组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildDropdown(builder, new DropdownModel
        {
            Icon = Icon,
            Items = UIConfig.Sizes,
            OnItemClick = OnSizeChangedAsync
        });
    }

    private async Task OnSizeChangedAsync(ActionInfo info)
    {
        Context.UserSetting.Size = info.Id;
        await Data.SaveUserSettingInfoAsync(Context.UserSetting);
        await JS.SetCurrentSizeAsync(info.Id);
        Navigation.Refresh(true);
    }
}