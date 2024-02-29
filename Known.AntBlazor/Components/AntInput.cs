namespace Known.AntBlazor.Components;

public class AntInput<TValue> : Input<TValue>, IAntField
{
    [Parameter] public int Span { get; set; }
    [Parameter] public string Label { get; set; }
    [Parameter] public bool Required { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Label))
            base.BuildRenderTree(builder);
        else
            builder.FormItem(this, b => base.BuildRenderTree(b));
    }
}