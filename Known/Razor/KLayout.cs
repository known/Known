using Known.Extensions;

namespace Known.Razor;

public class KLayout : BaseComponent
{
    internal bool IsDemo { get; set; }
    internal string Header { get; set; } = "header";
    internal string Sider { get; set; } = "sider";
    internal string Body { get; set; } = "body";

    [Parameter] public string Style { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsDemo)
        {
            BuildLayout(builder);
            return;
        }

        var info = Setting.Info;
        var css = CssBuilder.Default(Style).AddClass(info?.Layout).Build();
        builder.Div(css, attr =>
        {
            builder.Div(Header, attr =>
            {
                if (!string.IsNullOrWhiteSpace(info?.Layout))
                    attr.Style($"background-color:{info?.ThemeColor}");
                BuildHeader(builder);
            });
            builder.Div(Sider, attr =>
            {
                if (!string.IsNullOrWhiteSpace(info?.Layout))
                    attr.Style($"background-color:{info?.SiderColor}");
                else
                    attr.Style($"background-color:{info?.ThemeColor}");
                builder.Div("logo", attr =>
                {
                    attr.Style($"background-color:{info?.ThemeColor}");
                    builder.Img(attr => attr.Src("img/logo.png"));
                });
                BuildSider(builder);
            });
            builder.Div(Body, attr => BuildBody(builder));
        });
    }

    protected virtual void BuildHeader(RenderTreeBuilder builder) { }
    protected virtual void BuildSider(RenderTreeBuilder builder) { }
    protected virtual void BuildBody(RenderTreeBuilder builder) { }

    private void BuildLayout(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default(Style).Build();
        builder.Div(css, attr =>
        {
            builder.Div(Header);
            builder.Div(Sider, attr =>
            {
                builder.Div("logo", attr =>
                {
                    builder.Img(attr => attr.Src("img/logo.png"));
                });
            });
            builder.Div(Body);
        });
    }
}