namespace Known.AntBlazor;

public class DataItemValue : ComponentBase
{
    [Parameter] public DataItem Value { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(Value, ChildContent);
    }
}