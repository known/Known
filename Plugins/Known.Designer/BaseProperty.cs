namespace Known.Designer;

class BaseProperty<TModel> : BaseComponent where TModel : class, new()
{
    protected List<CodeInfo> controlTypes;
    protected List<CodeInfo> categories;

    [Parameter] public TModel Model { get; set; }
    [Parameter] public Action<TModel> OnChanged { get; set; }

    internal bool IsReadOnly => ReadOnly || Model == null;

    internal void SetModel(TModel model)
    {
        Model = model ?? new();
        StateChanged();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Model ??= new();
        controlTypes = Cache.GetCodes(nameof(FieldType)).Select(c => new CodeInfo(c.Name, c.Name)).ToList();
        categories = Cache.GetCodes(Constants.DicCategory);
    }

    protected override void BuildRender(RenderTreeBuilder builder) => BuildForm(builder);
    protected virtual void BuildForm(RenderTreeBuilder builder) { }

    protected void BuildPropertyItem(RenderTreeBuilder builder, string label, Action<RenderTreeBuilder> template)
    {
        builder.Div("item", () =>
        {
            builder.Label(Language[label]);
            template?.Invoke(builder);
        });
    }
}