namespace Known.AntBlazor.Components;

public class DataForm<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (Model.Header != null)
            builder.Fragment(Model.Header);
        builder.Component<AntForm<TItem>>()
               .Set(c => c.Class, Model.ClassName)
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
            builder.Component<AntFormRow<TItem>>().Set(c => c.Row, row).Build();
        }
    }
}