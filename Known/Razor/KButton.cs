namespace Known.Razor;

public class KButton : BaseComponent
{
    public KButton()
    {
        Id = Utils.GetGuid();
    }

    public static Action<RenderTreeBuilder, KButton> Render { get; set; }

    [Parameter] public StyleType Type { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        if (Render != null)
        {
            Render(builder, this);
            return;
        }

        var css = CssBuilder.Default(Type.ToString().ToLower()).AddClass(Style).Build();
        builder.Button(css, attr =>
        {
            attr.Id(Id).Disabled(!Enabled).OnClick(Callback(OnButtonClick));
            builder.IconName(Icon, Text);
        });
    }

    private void OnButtonClick()
    {
        UI.Enabled(Id, false);
        if (OnClick.HasDelegate)
        {
            var task = OnClick.InvokeAsync();
            if (task.IsCompleted)
            {
                UI.Enabled(Id, true);
            }
        }
    }
}