namespace KnownAntDesign.Renders;

class KaCaptchaRender : BaseRender<KCaptcha>
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Input<string>>(Component.Id)
               .Set(c => c.Type, "text")
               .Set(c => c.Prefix, BuildIcon)
               .Set(c => c.Suffix, BuildImage)
               .Set(c => c.Placeholder, Component.Placeholder)
               .Set(c => c.Value, Component.Value)
               .Set(c => c.ValueChanged, Component.Callback<string>(OnValueChanged))
               .Build();
        
        //BuildIcon(builder, Component.Icon);
        //builder.Input(attr =>
        //{
        //    attr.Type("text").Id(Component.Id).Name(Component.Id).Disabled(!Component.Enabled)
        //        .Value(Component.Value).Required(Component.Required)
        //        .Placeholder(Component.Placeholder)
        //        .Add("autocomplete", "off")
        //        .OnChange(Component.CreateBinder())
        //        .OnEnter(Component.OnEnter);
        //});
        BuildImage(builder);
    }

    private void BuildIcon(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Component.Icon))
            builder.Icon(Component.Icon);
    }

    private void BuildImage(RenderTreeBuilder builder)
    {
        builder.Canvas(attr => attr.Id(Component.CanvasId).Class("captcha").Title("点击图片刷新").OnClick(Component.Callback(Component.GenerateCode)));
    }

    private void OnValueChanged(string value)
    {
        Component.SetValue(value);
    }
}