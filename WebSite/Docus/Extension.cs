using WebSite.Data;

namespace WebSite.Docus;

static class Extension
{
    internal static void BuildList(this RenderTreeBuilder builder, string[] items) => builder.BuildList("", items);
    internal static void BuildList(this RenderTreeBuilder builder, string className, string[] items)
    {
        builder.Ul(className, attr =>
        {
            foreach (var item in items)
            {
                builder.Li(attr => builder.Text(item));
            }
        });
    }

    internal static void BuildDemo<T>(this RenderTreeBuilder builder, string title, string style = "") where T : BaseComponent
    {
        var type = typeof(T);
        var code = ComponentService.GetCode(type);
        builder.H3(title);
        builder.Div($"demo {style}", attr =>
        {
            builder.Div("view", attr => builder.Component<T>().Build());
            builder.Div("code", attr =>
            {
                builder.Element("pre", attr => builder.Element("code", attr => builder.Text(code)));
            });
        });
    }

    internal static void BuildDemo(this RenderTreeBuilder builder, string title, string code)
    {
        builder.H3(title);
        builder.Div("demo block", attr =>
        {
            builder.Div("code", attr =>
            {
                builder.Element("pre", attr => builder.Element("code", attr => builder.Text(code)));
            });
        });
    }
}