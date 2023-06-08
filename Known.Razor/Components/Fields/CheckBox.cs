namespace Known.Razor.Components.Fields;

public class CheckBox : Field
{
    [Parameter] public string Text { get; set; }

    protected override void BuildText(RenderTreeBuilder builder)
    {
        BuildRadio(builder, "checkbox", Text, "True", false, IsChecked);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        BuildRadio(builder, "checkbox", Text, "True", Enabled, IsChecked, (isCheck, value) =>
        {
            Value = isCheck ? "True" : "False";
        });
    }

    private bool IsChecked => Value == "True";
}