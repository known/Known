namespace Known.Razor;

public interface IButton : IBaseComponent
{
    StyleType Type { get; set; }
    string Icon { get; set; }
    string Text { get; set; }
    string Style { get; set; }
    EventCallback OnClick { get; set; }
}

public class KButton : BaseComponent, IButton
{
    public KButton()
    {
        Id = Utils.GetGuid();
    }

    [Parameter] public StyleType Type { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

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