namespace Known.AntBlazor.Components;

public class DataForm<TItem> : BaseForm where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        builder.Component<AntForm<TItem>>()
               .Set(c => c.Class, Model.Class)
               .Set(c => c.Form, Model)
               .Set(c => c.ChildContent, this.BuildTree<TItem>(BuildContent))
               .Build();
    }

    private void BuildContent(RenderTreeBuilder builder, TItem item)
    {
        if (Model.Rows == null || Model.Rows.Count == 0)
            return;

        foreach (var row in Model.Rows)
        {
            builder.Component<GridRow>()
                   .Set(c => c.ChildContent, b => BuildRow(b, row))
                   .Build();
        }
    }

    private void BuildRow(RenderTreeBuilder builder, FormRow<TItem> row)
    {
        var colSpan = 24 / row.Fields.Count;
        foreach (var field in row.Fields)
        {
            var label = Language?.GetString<TItem>(field.Column);
            var required = field.Column.Required;
            builder.Component<DataItem>()
                   .Set(c => c.Span, colSpan)
                   .Set(c => c.Label, label)
                   .Set(c => c.Required, required)
                   .Set(c => c.Rules, field.ToRules(Context))
                   .Set(c => c.ChildContent, b =>
                   {
                       b.Component<KField<TItem>>().Set(c => c.Model, field).Build();
                   })
                   .Build();
        }
    }
}