namespace WebSite.Docus.Inputs.Pickers;

class Picker1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Picker>("客户：", "Picker1")
               .Set(f => f.Pick, new CustomerList())
               .Build();
    }
}

class KmCustomer
{
    [Column("编码")] public string? Code { get; set; }
    [Column("名称")] public string? Name { get; set; }

    public override string ToString() => Name;
}

//在客户列表页面实现IPicker接口
class CustomerList : DataGrid<KmCustomer>, IPicker
{
    public CustomerList()
    {
        Name = string.Empty;//设为空不添加访问日志
    }

    public CustomerList(string separator) :this()
    {
        Separator = separator;
    }

    [Parameter] public string? Separator { get; set; }

    #region IPicker
    public string Title => "选择客户";

    public Size Size => new(500, 400);

    public void BuildPick(RenderTreeBuilder builder)
    {
        builder.Component<CustomerList>()
               .Set(c => c.Separator, Separator)
               .Set(c => c.OnPicked, OnPicked)
               .Build();
    }

    public override void OnRowDoubleClick(int row, KmCustomer item)
    {
        if (!string.IsNullOrWhiteSpace(Separator))
            OnPicked.Invoke($"{item.Code}{Separator}{item.Name}");
        else
            OnPicked?.Invoke(item);
        UI.CloseDialog();
    }
    #endregion

    protected override Task InitPageAsync()
    {
        //建造栏位
        var builder = new ColumnBuilder<KmCustomer>();
        builder.Field(r => r.Code);
        builder.Field(r => r.Name, true);
        Columns = builder.ToColumns();
        //测试数据
        Data = new List<KmCustomer> {
            new KmCustomer { Code = "C001", Name = "客户一" },
            new KmCustomer { Code = "C002", Name = "客户二" },
            new KmCustomer { Code = "C003", Name = "客户三" }
        };
        return base.InitPageAsync();
    }
}