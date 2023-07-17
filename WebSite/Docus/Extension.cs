using Known.Razor;
using Known.Razor.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace WebSite.Docus;

static class Extension
{
    internal static void BuildList(this RenderTreeBuilder builder, string[] items)
    {
        builder.Ul(attr =>
        {
            foreach (var item in items)
            {
                builder.Li(attr => builder.Text(item));
            }
        });
    }

    internal static void BuildDemo<T>(this RenderTreeBuilder builder, string title, string code) where T : BaseComponent
    {
        builder.H3(title);
        builder.Div("demo", attr =>
        {
            builder.Div("view", attr => builder.Component<T>().Build());
            builder.Div("code", attr =>
            {
                builder.Element("pre", attr => builder.Element("code", attr => builder.Text(code)));
            });
        });
    }
}