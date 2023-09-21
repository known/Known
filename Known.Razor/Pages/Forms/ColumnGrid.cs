namespace Known.Razor.Pages.Forms;

class ColumnGrid : EditGrid<ColumnInfo>
{
    private List<Type> modelTypes;
    private CodeInfo[] models;

    protected override void OnInitialized()
    {
        var builder = new ColumnBuilder<ColumnInfo>();
        builder.Field(r => r.Id).Name("ID").Edit().Width(100);
        builder.Field(r => r.Name).Name("名称").Edit().Width(100);
        builder.Field(r => r.Type).Name("类型").Edit(new SelectOption(typeof(ColumnType))).Width(100);
        builder.Field(r => r.Align).Name("对齐").Edit(new SelectOption(typeof(AlignType))).Width(100);
        builder.Field(r => r.Width).Name("宽度").Edit<Number>().Center(100);
        builder.Field(r => r.IsVisible).Name("显示").Edit();
        builder.Field(r => r.IsQuery).Name("查询").Edit();
        builder.Field(r => r.IsAdvQuery).Name("高级查询").Edit();
        builder.Field(r => r.IsFixed).Name("固定").Edit();
        builder.Field(r => r.IsSort).Name("排序").Edit();
        builder.Field(r => r.IsSum).Name("合计").Edit();
        Columns = builder.ToColumns();

        modelTypes = KRConfig.GetModelTypes();
        models = modelTypes.Select(t => new CodeInfo(t.FullName, t.Name)).ToArray();
        ActionHead = b =>
        {
            b.Link(Language.Add, Callback(OnAdd));
            b.Link("插入", Callback(OnInsert));
        };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);

        if (ReadOnly)
            return;

        builder.Div("tool", attr =>
        {
            builder.Field<Select>("实体模型：", "Model")
                   .Set(f => f.Items, models)
                   .Set(f => f.ValueChanged, OnModelChanged)
                   .Build();
        });
    }

    private void OnModelChanged(string value)
    {
        var type = modelTypes.FirstOrDefault(t => t.FullName == value);
        var attrs = TypeHelper.GetColumnAttributes(type);
        Data = attrs.Where(a => a.IsGrid).Select(a =>
        {
            var info = new ColumnInfo(a.Description, a.Property.Name);
            info.SetColumnType(a.Property.PropertyType);
            return info;
        }).ToList();
        StateChanged();
    }
}