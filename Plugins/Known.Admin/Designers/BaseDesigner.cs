namespace Known.Designers;

class BaseDesigner<TModel> : BaseComponent
{
    [Parameter] public SysModule Module { get; set; }
    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }
}

class BaseViewDesigner<TModel> : BaseDesigner<TModel>
{
    internal BaseView<TModel> view;
    internal List<FieldInfo> Fields { get; set; } = [];

    protected override void BuildRender(RenderTreeBuilder builder) => builder.Cascading(this, BuildTree);

    private void BuildTree(RenderTreeBuilder builder)
    {
        builder.Div($"kui-designer {typeof(TModel).Name}", () =>
        {
            builder.Div("panel-model", () =>
            {
                builder.Component<ColumnPanel<TModel>>()
                       .Set(c => c.ReadOnly, ReadOnly)
                       .Set(c => c.OnFieldCheck, OnFieldCheck)
                       .Set(c => c.OnFieldClick, OnFieldClick)
                       .Build();
            });
            builder.Div("panel-view", () => BuildView(builder));
            builder.Div("panel-property", () => BuildProperty(builder));
        });
    }

    protected virtual void BuildView(RenderTreeBuilder builder) { }
    protected virtual void BuildProperty(RenderTreeBuilder builder) { }
    protected virtual void OnFieldCheck(FieldInfo field) { }
    protected virtual void OnFieldClick(FieldInfo field) { }
}