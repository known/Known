namespace Known.Internals;

[NavPlugin("字体大小", "font-size", Category = "组件", Sort = 3)]
class NavFontSize : BaseNav
{
    protected override string Icon => "font-size";

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Dropdown(new DropdownModel
        {
            Icon = Icon,
            Items = UIConfig.Sizes,
            OnItemClick = OnSizeChangedAsync
        });
    }

    private async Task OnSizeChangedAsync(ActionInfo info)
    {
        Context.UserSetting.Size = info.Id;
        await Admin.SaveUserSettingAsync(Context.UserSetting);
        await JS.SetCurrentSizeAsync(info.Id);
    }
}