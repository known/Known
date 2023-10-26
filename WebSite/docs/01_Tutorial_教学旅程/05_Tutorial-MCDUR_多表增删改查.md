# 多表增删改查示例

本章介绍学习多张表增、删、改、查功能如何实现，下面以销货出库单作为示例，该业务栏位如下：

> **销货出库单栏位**
> - 销货单号、销货日期、状态、客户、备注
>
> **销货出库单明细栏位**
> - 商品编码、商品名称、规格型号、数量、单位、单价、金额

该示例适用于出货明细数量较小情况，单据表头和表体组合查询和提交。

对于出货明细数量较大的情况，建议表头与表体分开查询和提交，表体采用分页查询。

## 1. 前后端共用

### 1.1. 创建实体类

- 在KIMS项目Entities文件夹下创建KmBill和KmBillList实体类
- 该类继承EntityBase类
- 属性使用Column特性描述，用于生成页面字段和数据校验

```csharp
//销货出库单
public class KmBill : EntityBase
{
    [Column("销货单号", "", true, "1", "50")]
    public string? BillNo { get; set; }
    ......
    [Column("客户", "", true, "1", "50")]
    public string? BillDate { get; set; }
    [Column("备注", "", false)]
    public string? Note { get; set; }

    //出货明细，为了降低代码量，扩展实体类，不再创建DTO类
    //此处使用虚属性，ORM映射SQL时忽略该属性
    public virtual List<KmBillList>? Lists { get; set; }
}
//销货出库单明细
public class KmBillList : EntityBase
{
    [Column("销货单ID", "", true, "1", "50")]
    public string? HeadId { get; set; }
    [Column("商品编码", "", true, "1", "50")]
    public string? Code { get; set; }
    [Column("数量", "", false)]
    public decimal? Qty { get; set; }
    [Column("单价", "", false)]
    public decimal? Price { get; set; }
    [Column("金额", "", false)]
    public decimal? Amount { get; set; }

    //如下虚属性用于关联商品资料表查询显示
    public virtual string? Name { get; set; }  //商品名称
    public virtual string? Model { get; set; } //规格型号
    public virtual string? Unit { get; set; }   //计量单位
}
```

### 1.2. 创建Client类

- 在KIMS项目Clients文件夹下创建BillClient类
- 该类是前后端数据交互接口，继承ClientBase类
- 该类只需提供分页查询、删除和保存，导入功能由框架统一异步处理

```csharp
public class BillClient : ClientBase
{
    public BillClient(Context context) : base(context) { }

    public Task<PagingResult<KmBill>> QueryBillsAsync(PagingCriteria criteria) => Context.QueryAsync<KmBill>("Bill/QueryBills", criteria);
    public Task<Result> DeleteBillsAsync(List<KmBill> models) => Context.PostAsync("Bill/DeleteBills", models);
    public Task<KmBill> GetBillAsync(string id) => Context.GetAsync<KmBill>($"Bill/GetBill?id={id}");
    public Task<Result> SaveBillAsync(KmBill model) => Context.PostAsync("Bill/SaveBill", model);
}
```

- 在KIMS项目Clients\ClientFactory类中添加BillClient类的实例

```csharp
public class ClientFactory
{
    public ClientFactory(Context context)
    {
        Bill = new BillClient(context);
    }

    public BillClient Bill { get; }
}
```

## 2. 前端

### 2.1. 创建List页面

- 在KIMS.Razor项目BillData文件夹下创建BillList类
- 该类是数据列表页面，继承WebGridView<KmBill, BillForm>类
- 列表页面按钮和栏位在框架模块管理中配置

```csharp
class BillList : WebGridView<KmBill, BillForm>
{
    protected override Task InitPageAsync()
    {
        //表格栏位格式化显示
        //销货单号链接，点击显示销货单查看表单
        Column(c => c.BillNo).Template((b, r) => b.Link(r.BillNo, Callback(() => View(r))));
        Column(c => c.Status).Template(BillStatusCell);//显示状态标签
        return base.InitPageAsync();
    }

    //分页查询
    protected override Task<PagingResult<KmBill>> OnQueryData(PagingCriteria criteria)
    {
        return Client.Bill.QueryBillsAsync(criteria);
    }
    
    public void New() => ShowForm();//新增按钮方法
    public void DeleteM() => DeleteRows(Client.Bill.DeleteBillsAsync);//批量删除按钮方法
    public void Edit(KmBill row) => ShowForm(row);//编辑操作方法
    public void Delete(KmBill row) => DeleteRow(row, Client.Bill.DeleteBillsAsync);//删除操作方法
}
```

### 2.2. 创建Form页面

- 在KIMS.Razor项目BillData\Forms文件夹下创建BillForm类
- 该类是数据编辑和查看明细页面，继承WebForm<KmBill>类

```csharp
[Dialog(980, 580)]//设置对话框大小
class BillForm : WebForm<KmBill>
{
    private KmBill? head;
    //初始化表单，查询表头表体组合数据
    protected override async Task InitFormAsync()
    {
        var model = TModel;
        head = await Client.Bill.GetBillAsync(model.Id);
    }
    //表单布局
    protected override void BuildFields(FieldBuilder<KmBill> builder)
    {
        builder.Hidden(f => f.Id);//隐藏字段
        builder.Table(table =>
        {
            table.ColGroup(11, 25, 11, 25, 11, 17);
            table.Tr(attr =>
            {
                //销货单号不可编辑，自动生成编号
                table.Field<Text>(f => f.BillNo).Enabled(false).Build();
                table.Field<Date>(f => f.BillDate).Build();
                table.Field<Text>(f => f.Status).Enabled(false).Build();
            });
            table.Tr(attr => table.Field<TextArea>(f => f.Note).ColSpan(5).Build());
            builder.FormList<BillListGrid>("商品明细", "", attr =>
            {
                attr.Set(c => c.ReadOnly, ReadOnly)
                    .Set(c => c.Data, head?.Lists);//设置表体数据
            });
        });
    }
    //表单底部按钮
    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }
    //保存按钮方法
    private void OnSave()
    {
        SubmitAsync(data =>
        {
            head?.FillModel(data);//自动填充表单修改数据
            return Client.Bill.SaveBillAsync(head);
        });
    }
}
```

### 2.3. 创建表体页面

- 在KIMS.Razor项目BillData\Forms文件夹下创建BillListGrid类
- 该类是表体数据编辑表格，继承EditGrid<KmBillList>类

```csharp
//可编辑表体组件
class BillListGrid : EditGrid<KmBillList>
{
    public BillListGrid()
    {
        OrderBy = nameof(KmBillList.ItemNo);//默认排序
        Name = "商品明细";
    }
    //初始化表格栏位
    protected override Task OnInitializedAsync()
    {
        //如下栏位有Edit方法为可编辑栏位，否则不可编辑
        var builder = new ColumnBuilder<KmBillList>();
        //商品库存选择器，弹出对话框查询商品库存，双击选择要出库的商品
        builder.Field(r => r.Code).Edit(new GoodsStock(), OnCodeChanged);
        builder.Field(r => r.Name);//不可编辑
        builder.Field(r => r.Model);
        builder.Field(r => r.Qty).Edit<Number>(OnQtyChanged);//可编辑
        builder.Field(r => r.Unit);
        builder.Field(r => r.Price).Edit<Number>(OnPriceChanged);
        builder.Field(r => r.Amount).IsSum().Edit<Number>(OnAmountChanged);
        builder.Field(r => r.Note).Edit();
        Columns = builder.ToColumns();
        return base.OnInitializedAsync();
    }
    //切换商品编码，自动带出商品信息
    private void OnCodeChanged(KmBillList row, object value)
    {
        var g = value as StockInfo;
        row.Type = g?.Type;
        row.Code = g?.Code;
        row.Name = g?.Name;
        row.Model = g?.Model;
        row.Unit = g?.Unit;
    }
    //更改数量，自动计算金额
    private void OnQtyChanged(KmBillList row, object value)
    {
        var qty = Utils.ConvertTo<decimal>(value);
        row.Amount = Utils.Round(qty * (row.Price ?? 0), 2);
    }
    //更改单价，自动计算金额
    private void OnPriceChanged(KmBillList row, object value)
    {
        var price = Utils.ConvertTo<decimal>(value);
        row.Amount = Utils.Round(price * row.Qty, 2);
    }
    //更改金额，自动计算单价
    private void OnAmountChanged(KmBillList row, object value)
    {
        var amount = Utils.ConvertTo<decimal>(value);
        row.Price = row.Qty == 0 ? 0 : Utils.Round(amount / row.Qty, 2);
    }
}
```

## 3. 后端

### 3.1. 创建Controller类

- 在KIMS.Core项目Controllers文件夹下创建BillController类
- 该类为服务端WebApi，继承BaseController类

```csharp
[Route("[controller]")]
public class BillController : BaseController
{
    private BillService Service => new(Context);

    [HttpPost("[action]")]
    public PagingResult<KmBill> QueryBills([FromBody] PagingCriteria criteria) => Service.QueryBills(criteria);

    [HttpPost("[action]")]
    public Result DeleteBills([FromBody] List<KmBill> models) => Service.DeleteBills(models);

    [HttpGet("[action]")]
    public KmBill GetBill([FromQuery] string id) => Service.GetBill(id);
    
    [HttpPost("[action]")]
    public Result SaveBill([FromBody] KmBill model) => Service.SaveBill(model);
}
```

### 3.2. 创建Service类

- 在KIMS.Core项目Services文件夹下创建BillService类
- 该类为业务逻辑服务类，继承ServiceBase类

```csharp
class BillService : ServiceBase
{
    internal BillService(Context context) : base(context) { }
    //分页查询
    internal PagingResult<KmBill> QueryBills(PagingCriteria criteria)
    {
        return BillRepository.QueryBills(Database, criteria);
    }
    //删除数据
    internal Result DeleteBills(List<KmBill> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        //此处增加删除数据校验
        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in models)
            {
                //删除表体
                BillRepository.DeleteBillLists(db, item.Id);
                db.Delete(item);
            }
        });
    }
    //获取组合数据
    internal KmBill GetBill(string id)
    {
        if (string.IsNullOrEmpty(id))//id为空返回默认值
            return GetDefaultBill();

        var entity = Database.QueryById<KmBill>(id);
        if (entity == null)//为空返回默认值
            entity = GetDefaultBill();
        else//否则组装表体数据返回
            entity.Lists = BillRepository.GetBillLists(Database, id);
        return entity;
    }
    //保存数据
    internal Result SaveBill(KmBill model)
    {
        if (model == null)
            return Result.Error("不能提交空数据！");

        var vr = model.Validate();//验证输入栏位
        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            if (model.IsNew)
                model.BillNo = GetBillMaxNo(db);

            //清空表体合计数据
            model.TotalAmount = 0;
            model.GoodsName = string.Empty;
            //先删除表体，再插入表体
            BillRepository.DeleteBillLists(db, model.Id);
            if (model.Lists != null && model.Lists.Count > 0)
            {
                var index = 0;
                var lists = new List<KmBillList>();
                foreach (var item in model.Lists)
                {
                    item.HeadId = model.Id;
                    item.ItemNo = ++index;
                    db.Insert(item);
                    lists.Add(item);
                }
                //合计表体数据
                model.TotalAmount = lists.Sum(l => l.Amount);
                model.GoodsName = string.Join(",", lists.Select(l => l.Name));
            }
            db.Save(model);
        }, model);
    }
    //获取默认表头
    private KmBill GetDefaultBill()
    {
        return new KmBill
        {
            BillNo = GetBillMaxNo(Database),
            BillDate = DateTime.Now,
            Status = "暂存",
            Lists = new List<JxBillList>()
        };
    }
    //获取销货单最大编号
    private static string GetBillMaxNo(Database db)
    {
        var prefix = $"S{DateTime.Now:yyyy}";
        var maxNo = BillRepository.GetBillMaxNo(db, prefix);
        if (string.IsNullOrWhiteSpace(maxNo))
            maxNo = $"{prefix}00000";
        return GetMaxFormNo(prefix, maxNo);
    }
}
```

### 3.3. 创建Repository类

- 在KIMS.Core项目Repositories文件夹下创建BillRepository类
- 该类为数据访问类

```csharp
class BillRepository
{
    //Head
    //分页查询
    internal static PagingResult<KmBill> QueryBills(Database db, PagingCriteria criteria)
    {
        var sql = "select * from KmBill where CompNo=@CompNo";
        return db.QueryPage<KmBill>(sql, criteria);//查询条件自动绑定
    }
    //获取销货单最大编号
    internal static string GetBillMaxNo(Database db, string prefix)
    {
        var sql = $"select max(BillNo) from KmBill where CompNo=@CompNo and BillNo like '{prefix}%'";
        return db.Scalar<string>(sql, new { db.User.CompNo });
    }
    //List
    //根据表头ID获取表体数据
    internal static List<KmBillList> GetBillLists(Database db, string headId)
    {
        //关联商品资料表查询商品信息
        var sql = "select a.*,b.Name,b.Model,b.Unit from KmBillList a,KmGoods b where a.Code=b.Code and HeadId=@headId";
        return db.QueryList<KmBillList>(sql, new { headId });
    }
    //根据表头ID删除表体数据
    internal static void DeleteBillLists(Database db, string headId)
    {
        var sql = "delete from KmBillList where HeadId=@headId";
        db.Execute(sql, new { headId });
    }
}
```

## 4. 运行测试

- 运行效果如下

![输入图片说明](https://foruda.gitee.com/images/1690379769309759337/4c52bcad_14334.png "屏幕截图")
![输入图片说明](https://foruda.gitee.com/images/1690379629202890739/e2184c3d_14334.png "屏幕截图")
