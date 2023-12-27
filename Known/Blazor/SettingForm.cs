using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class SettingForm : BaseForm<SettingInfo>
{
    protected override async Task OnInitFormAsync()
    {
        Model = new FormModel<SettingInfo>(UI)
        {
            LabelSpan = 10,
            Data = Context.UserSetting
        };
        await base.OnInitFormAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div("kui-form-setting", () =>
        {
            base.BuildRenderTree(builder);
            builder.Div("center", () =>
            {
                UI.Button(builder, "保存", this.Callback<MouseEventArgs>(SaveAsync), "primary");
                UI.Button(builder, "重置", this.Callback<MouseEventArgs>(ResetAsync));
            });
        });
    }

    private async void SaveAsync(MouseEventArgs arg)
    {
        var result = await Platform.Setting.SaveSettingAsync(SettingInfo.KeyInfo, Model.Data);
        if (result.IsValid)
        {
            Context.UserSetting = Model.Data;
            Context.RefreshPage();
        }
    }

    private async void ResetAsync(MouseEventArgs arg)
    {
        var result = await Platform.Setting.DeleteUserSettingAsync(SettingInfo.KeyInfo);
        if (result.IsValid)
        {
            Model.Data = new();
            Context.UserSetting = Model.Data;
            Context.RefreshPage();
        }
    }
}