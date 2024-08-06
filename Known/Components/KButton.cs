namespace Known.Components;

public class KButton : BaseComponent
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public ActionInfo Action { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        Action ??= new ActionInfo
        {
            Id = Id,
            Name = Name,
            Icon = Icon,
            Style = Style,
            Enabled = Enabled,
            Visible = Visible,
            OnClick = OnClick
        };
        UI.BuildButton(builder, Action);
    }
}