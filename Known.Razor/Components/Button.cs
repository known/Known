namespace Known.Razor.Components;

public class Button : BaseComponent
{
    private readonly string id;

    public Button()
    {
        id = Utils.GetGuid();
    }

    [Parameter] public string Style { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public EventCallback OnClick { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Visible)
            return;

        builder.Button(Style, attr =>
        {
            attr.Id(id).Disabled(!Enabled).OnClick(Callback(OnButtonClick));
            builder.IconName(Icon, Text);
        });
    }

    private void OnButtonClick()
    {
        UI.Enabled(id, false);
        if (OnClick.HasDelegate)
        {
            var task = OnClick.InvokeAsync();
            if (task.IsCompleted)
            {
                UI.Enabled(id, true);
            }
        }
    }
}