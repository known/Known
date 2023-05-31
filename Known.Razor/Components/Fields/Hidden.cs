namespace Known.Razor.Components.Fields;

public class Hidden : Field
{
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.Input(attr => attr.Type("hidden").Name(Id).Value(Value));
}