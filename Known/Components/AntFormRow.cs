namespace Known.Components;

class AntFormRow<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public FormRow<TItem> Row { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Row == null)
            return;

        builder.Component<AntRow>()
               .Set(c => c.ChildContent, BuildDataRow)
               .Build();
    }

    private void BuildDataRow(RenderTreeBuilder builder)
    {
        var colSpan = 24 / Row.Fields.Count;
        foreach (var field in Row.Fields)
        {
            var column = field.Column;
            var label = column.Label ?? Language?.GetString<TItem>(column);
            builder.Component<DataItem>()
                   .Set(c => c.Span, column.Span ?? colSpan)
                   .Set(c => c.Label, label)
                   .Set(c => c.Required, column.Required)
                   .Set(c => c.Rules, field.ToRules(Context))
                   .Set(c => c.ChildContent, b => b.Component<KField<TItem>>().Set(c => c.Model, field).Build())
                   .Build();
        }
    }
}