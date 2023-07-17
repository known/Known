namespace Known.Test.Pages;

static class Extension
{
    internal static void BuildDemo(this RenderTreeBuilder builder, string text, Action action)
    {
        builder.Div("demo-row", attr =>
        {
            builder.Div("demo-caption", text);
            action();
        });
    }
}