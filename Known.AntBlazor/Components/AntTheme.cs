namespace Known.AntBlazor.Components;

public class AntTheme : BaseComponent
{
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<Switch>()
               .Set(c => c.CheckedChildren, "🌜")
               .Set(c => c.UnCheckedChildren, "🌞")
               .Set(c => c.Value, Context.Theme == "dark")
               .Set(c => c.OnChange, this.Callback<bool>(ThemeChanged))
               .Build();
    }

    private async void ThemeChanged(bool isDark)
    {
        Context.Theme = isDark ? "dark" : "light";
        await JS.SetCurrentThemeAsync(Context.Theme);
    }
}