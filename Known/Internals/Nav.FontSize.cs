namespace Known.Internals;

[NavPlugin(Language.NavFontSize, "font-size", Category = Language.Component, Sort = 3)]
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