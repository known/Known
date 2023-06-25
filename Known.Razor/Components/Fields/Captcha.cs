namespace Known.Razor.Components.Fields;

public class Captcha : Field
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        BuildIcon(builder, Icon);
        builder.Input(attr =>
        {
            attr.Type("text").Id(Id).Name(Id).Disabled(!Enabled)
                .Value(Value).Required(Required)
                .Placeholder(Placeholder)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder())
                .OnEnter(OnEnter);
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
        builder.Img(attr =>
        {
            var url = $"{Context.Http.BaseAddress}System/Captcha";
            var refresh = $"this.src='{url}?_='+Math.random()";
            attr.Class("captcha").Title("点击图片刷新").Src(url).Add("onclick", refresh);
        });
    }
}