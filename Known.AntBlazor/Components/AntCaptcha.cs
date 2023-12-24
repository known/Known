using AntDesign;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.AntBlazor.Components;

public class AntCaptcha : Input<string>
{
    private const string title = "点击图片刷新";

    public AntCaptcha()
    {
        Class = "ant-captcha";
    }

    [Parameter] public string ImgUrl { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.OpenElement("img").Src(ImgUrl).Title(title).OnClick("alert(this.src);").CloseElement();
    }
}