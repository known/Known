namespace Known.AntBlazor.Components;

public class AntTheme : BaseComponent
{
    [Parameter] public string Theme { get; set; }
    [Parameter] public Action<string> OnChanged { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<Switch>()
               .Set(c => c.CheckedChildren, "🌜")
               .Set(c => c.UnCheckedChildren, "🌞")
               .Set(c => c.Value, Theme == "dark")
               .Set(c => c.OnChange, this.Callback<bool>(ThemeChanged))
               .Build();
    }

    private async void ThemeChanged(bool isDark)
    {
        Theme = isDark ? "dark" : "light";
        await JS.SetCurrentThemeAsync(Theme);
        OnChanged?.Invoke(Theme);
    }
}