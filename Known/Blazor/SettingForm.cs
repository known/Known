using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class SettingForm : BaseComponent
{
    private FormModel<SettingInfo> model;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        model = new FormModel<SettingInfo>(UI);
        model.Data = Context.UserSetting;
        model.LabelSpan = 10;
        model.AddRow().AddColumn(c => c.IsLight);
        model.AddRow().AddColumn(c => c.Accordion);
        model.AddRow().AddColumn(c => c.MultiTab);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-setting", () =>
        {
            UI.BuildForm(builder, model);
            builder.Div("center", () =>
            {
                UI.BuildButton(builder, new ActionInfo
                {
                    Style = "primary",
                    Name = "保存",
                    OnClick = Callback<MouseEventArgs>(SaveAsync)
                });
                UI.BuildButton(builder, new ActionInfo
                {
                    Name = "重置",
                    OnClick = Callback<MouseEventArgs>(ResetAsync)
                });
            });
        });
    }

    private async void SaveAsync(MouseEventArgs arg)
    {
        var result = await Platform.Setting.SaveSettingAsync(SettingInfo.KeyInfo, model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = model.Data;
            Context.RefreshPage();
        }
    }

    private async void ResetAsync(MouseEventArgs arg)
    {
        var result = await Platform.Setting.DeleteUserSettingAsync(SettingInfo.KeyInfo);
        if (result.IsValid)
        {
            model.Data = new();
            Context.UserSetting = model.Data;
            Context.RefreshPage();
        }
    }
}