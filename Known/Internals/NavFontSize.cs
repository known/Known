﻿namespace Known.Internals;

[NavItem]
class NavFontSize : BaseNav
{
    protected override string Icon => "font-size";

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        UIConfig.Sizes.ForEach(s => s.Name = Language[$"Nav.Size{s.Id}"]);
    }

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
        await Platform.SaveUserSettingAsync(Context.UserSetting);
        await JS.SetCurrentSizeAsync(info.Id);
        Navigation.Refresh(true);
    }
}