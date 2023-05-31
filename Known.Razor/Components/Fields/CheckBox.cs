namespace Known.Razor.Components.Fields;

public class CheckBox : Field
{
    [Parameter] public string Text { get; set; }
    [Parameter] public bool Checked { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.Label(attr =>
        {
            attr.For(Id);
            builder.Input(attr =>
            {
                attr.Type("checkbox").Id(Id).Name(Id).Role("switch")
                    .Disabled(!Enabled).Required(Required).Checked(IsChecked)
                    .OnChange(EventCallback.Factory.CreateBinder<bool>(this, isCheck =>
                    {
                        Value = isCheck ? "True" : "False";
                        OnValueChange();
                    }, IsChecked));
            });
            builder.Text(Text);
        });
    }

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