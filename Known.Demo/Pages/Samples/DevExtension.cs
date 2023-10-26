namespace Known.Demo.Pages.Samples;

static class DevExtension
{
    internal static void BuildDemo(this RenderTreeBuilder builder, string text, Action action)
    {
        builder.Div("demo-row", attr =>
        {
            builder.Div("demo-caption", text);
            builder.Div("row", attr => action());
        });
    }
}