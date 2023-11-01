namespace Known.Renders;

class KCaptchaRender : BaseRender<KCaptcha>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Component.Icon);
        builder.Input(attr =>
        {
            attr.Type("text").Id(Component.Id).Name(Component.Id).Disabled(!Component.Enabled)
                .Value(Component.Value).Required(Component.Required)
                .Placeholder(Component.Placeholder)
                .Add("autocomplete", "off")
                .OnChange(Component.CreateBinder())
                .OnEnter(Component.OnEnter);
        });
        BuildImage(builder);
    }

    private static void BuildIcon(RenderTreeBuilder builder, string icon)
    {
        if (!string.IsNullOrWhiteSpace(icon))
            builder.Icon(icon);
    }

    private void BuildImage(RenderTreeBuilder builder)
    {
        builder.Canvas(attr => attr.Id(Component.CanvasId).Class("captcha").Title("点击图片刷新").OnClick(Component.Callback(Component.GenerateCode)));
    }
}