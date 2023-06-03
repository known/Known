namespace Known.Razor.Components.Fields;

public class Password : Field
{
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Placeholder { get; set; }
    [Parameter] public string OnEnter { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Icon))
            builder.Icon(Icon);
        builder.Input(attr =>
        {
            attr.Type("password").Id(Id).Name(Id).Placeholder(Placeholder).Value(Value)
                .Disabled(!Enabled).Required(Required).Readonly(ReadOnly).OnChange(CreateBinder());
        });
    }
}