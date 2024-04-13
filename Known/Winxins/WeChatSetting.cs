using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Winxins;

class WeChatSetting : BaseEditForm<WeixinInfo>
{
    protected override async Task OnInitFormAsync()
    {
        var data = await Platform.Weixin.GetWeixinAsync();
        data ??= new WeixinInfo();
        Model = new FormModel<WeixinInfo>(Context)
        {
            IsView = true,
            LabelSpan = 4,
            WrapperSpan = 10,
            Data = data
        };
        Model.AddRow().AddColumn(c => c.IsWeixinAuth);
        await base.OnInitFormAsync();
    }

    protected override void BuildFormContent(RenderTreeBuilder builder)
    {
        builder.FormPage(() =>
        {
            UI.BuildForm(builder, Model);
            builder.FormButton(() => BuildAction(builder));
        });
    }

    protected override Task<Result> OnSaveAsync(WeixinInfo model)
    {
        return Platform.Weixin.SaveWeixinAsync(model);
    }
}