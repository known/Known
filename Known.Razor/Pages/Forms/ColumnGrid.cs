namespace Known.Razor.Pages.Forms;

class ColumnGrid : EditGrid<ColumnInfo>
{
    private List<Type> modelTypes;
    private CodeInfo[] models;

    [Parameter] public bool IsModule { get; set; }
    [Parameter] public Action<List<ColumnInfo>> OnSetting { get; set; }

    protected override void OnInitialized()
    {
        if (IsModule)
        {
            modelTypes = KRConfig.GetModelTypes();
            models = modelTypes.Select(t => new CodeInfo(t.FullName, t.Name)).ToArray();
            ActionHead = b =>
            {
                b.Link(Language.Add, Callback(OnAdd), "bg-primary");
                b.Link("插入", Callback(OnInsert), "bg-primary");
            };
        }
        else
        {
            Style = "form-grid";
            ActionHead = null;
            Actions = new List<ButtonInfo> { GridAction.MoveUp, GridAction.MoveDown };
        }
        Columns = GetColumns(IsModule);
    }

    protected override void BuildOther(RenderTreeBuilder builder)
    {
        if (IsModule)
        {
            if (ReadOnly)
                return;

            builder.Div("tool inline", attr =>
            {
                builder.Span("实体模型：");
                builder.Field<Select>("Model").IsInput(true)
                       .Set(f => f.Items, models)
                       .Set(f => f.ValueChanged, OnModelChanged)
                       .Build();
            });
        }
        else
        {
            builder.Div("form-button", attr =>
            {
                builder.Button(FormButton.OK, Callback(OnOK));
                builder.Button(FormButton.Cancel, Callback(OnCancel));
            });
        }
    }

    private static List<Column<ColumnInfo>> GetColumns(bool isModule)
    {
        var builder = new ColumnBuilder<ColumnInfo>();
        if (isModule)
            builder.Field(r => r.Id).Name("ID").Edit().Width(100);
        builder.Field(r => r.Name).Name("名称").Edit();
        if (isModule)
            builder.Field(r => r.Type).Name("类型").Edit().Width(100).Select(new SelectOption(typeof(ColumnType)));
        builder.Field(r => r.Align).Name("对齐").Edit().Width(100).Select(new SelectOption(typeof(AlignType)));
        builder.Field(r => r.Width).Name("宽度").Edit().Control<Number>().Center(100);
        builder.Field(r => r.IsVisible).Name("显示").Edit();
        builder.Field(r => r.IsQuery).Name("查询").Edit();
        if (isModule)
            builder.Field(r => r.IsFixed).Name("固定").Edit();
        builder.Field(r => r.IsSort).Name("排序").Edit();
        if (isModule)
            builder.Field(r => r.IsSum).Name("合计").Edit();
        return builder.ToColumns();
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

    private void OnOK()
    {
        OnSetting?.Invoke(Data);
        OnCancel();
    }

    private void OnCancel() => UI.CloseDialog();
}