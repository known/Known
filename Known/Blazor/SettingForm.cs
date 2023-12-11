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
        model = new FormModel<SettingInfo>(UI)
        {
            LabelSpan = 10,
            Data = Context.UserSetting
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("form-setting", () =>
        {
            UI.BuildForm(builder, model);
            builder.Div("center", () =>
            {
                UI.Button(builder, "保存", this.Callback<MouseEventArgs>(SaveAsync), "primary");
                UI.Button(builder, "重置", this.Callback<MouseEventArgs>(ResetAsync));
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