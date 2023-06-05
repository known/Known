namespace Known.Razor.Components.Fields;

public class CheckBox : Field
{
    [Parameter] public string Text { get; set; }
    [Parameter] public bool Checked { get; set; }

    protected override void BuildChildText(RenderTreeBuilder builder) => BuildRadio(builder, "checkbox", Text, "True", false, IsChecked);

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildRadio(builder, "checkbox", Text, "True", Enabled, IsChecked, (isCheck, value) =>
        {
            Value = isCheck ? "True" : "False";
        });
    }

    private bool IsChecked => Checked || Value == "True";
}