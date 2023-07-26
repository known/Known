# 单表增删改查导示例

本章介绍学习单张表增、删、改、查、导功能如何实现，下面以商品资料作为示例，该业务栏位如下：

> 类型、编码、名称、规格、单位、库存下限、库存上限、备注

## 1. 前后端共用

### 1.1. 创建实体类

- 在KIMS项目Entities文件夹下创建KmGoods实体类
- 该类继承EntityBase类
- 属性使用Column特性描述，用于生成页面字段和数据校验

```csharp
public class KmGoods : EntityBase
{
    [Column("商品类型", "", true, "1", "50")]
    public string? Type { get; set; }
	......
    [Column("库存下限", "", false)]
    public decimal? MinStock { get; set; }
	......
    [Column("备注", "", false)]
    public string? Note { get; set; }
}
```

### 1.2. 创建Client类

- 在KIMS项目Clients文件夹下创建GoodsClient类
- 该类是前后端数据交互接口，继承ClientBase类
- 该类只需提供分页查询、删除和保存，导入功能由框架统一异步处理

```csharp
public class GoodsClient : ClientBase
{
    public GoodsClient(Context context) : base(context) { }

    public Task<PagingResult<KmGoods>> QueryGoodsesAsync(PagingCriteria criteria) => Context.QueryAsync<KmGoods>("Goods/QueryGoodses", criteria);
    public Task<Result> DeleteGoodsesAsync(List<KmGoods> models) => Context.PostAsync("Goods/DeleteGoodses", models);
    public Task<Result> SaveGoodsAsync(object model) => Context.PostAsync("Goods/SaveGoods", model);
}
```

## 2. 前端

### 2.1. 创建List页面

- 在KIMS.Razor项目BaseData文件夹下创建GoodsList类
- 该类是数据列表页面，继承WebGridView<KmGoods, GoodsForm>类
- 列表页面按钮和栏位在框架模块管理中配置

```csharp
class GoodsList : WebGridView<KmGoods, GoodsForm>
{
    protected override Task InitPageAsync()
    {
        //表格栏位格式化显示
        Column(c => c.Type).Select(new SelectOption { Codes = AppDictionary.GoodsType });
        Column(c => c.TaxRate).Template((b, r) => b.Text(r.TaxRate?.ToString("P")));
        return base.InitPageAsync();
    }

    //分页查询
    protected override Task<PagingResult<KmGoods>> OnQueryData(PagingCriteria criteria)
    {
        return Client.Goods.QueryGoodsesAsync(criteria);
    }
    
    public void New() => ShowForm();//新增按钮方法
    public void DeleteM() => DeleteRows(Client.Goods.DeleteGoodsesAsync);//批量删除按钮方法
    public void Edit(KmGoods row) => ShowForm(row);//编辑操作方法
    public void Delete(KmGoods row) => DeleteRow(row, Client.Goods.DeleteGoodsesAsync);//删除操作方法
}
```

### 2.2. 创建Form页面

- 在KIMS.Razor项目BaseData\Forms文件夹下创建GoodsForm类
- 该类是数据编辑和查看明细页面，继承WebForm<KmGoods>类

```csharp
[Dialog(800, 420)]//设置对话框大小
class GoodsForm : WebForm<KmGoods>
{
    //表单布局
    protected override void BuildFields(FieldBuilder<KmGoods> builder)
    {
        builder.Hidden(f => f.Id);//隐藏字段
        builder.Table(table =>
        {
            table.ColGroup(15, 35, 15, 35);
            table.Tr(attr =>
            {
                table.Field<Text>(f => f.Code).Enabled(TModel.IsNew).Build();//编码，编辑时灰显
                table.Field<Text>(f => f.Name).Build();
            });
            table.Tr(attr =>
            {
                table.Field<Select>(f => f.Type).Set(f => f.Codes, AppDictionary.GoodsType).Build();//下拉框
                table.Field<Select>(f => f.Unit).Set(f => f.Codes, AppDictionary.GoodsUnit).Build();
            });
            table.Tr(attr => table.Field<Text>(f => f.Model).ColSpan(3).Build());
            table.Tr(attr => table.Field<RadioList>(f => f.TaxRate).ColSpan(3).Set(f => f.Items, AppDictionary.TaxRates).Build());//单选按钮
            table.Tr(attr =>
            {
                table.Field<Number>(f => f.MinStock).Build();//数值框
                table.Field<Number>(f => f.MaxStock).Build();
            });
            table.Tr(attr => table.Field<TextArea>(f => f.Note).ColSpan(3).Build());//文本域
        });
    }
    //表单底部按钮
    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }
    //保存按钮方法
    private void OnSave() => SubmitAsync(Client.Goods.SaveGoodsAsync);
}
```

## 3. 后端

### 3.1. 创建Controller类

- 在KIMS.Core项目Controllers文件夹下创建GoodsController类
- 该类为服务端WebApi，继承BaseController类

```csharp
[Route("[controller]")]
public class GoodsController : BaseController
{
    private GoodsService Service => new(Context);

    [HttpPost("[action]")]
    public PagingResult<KmGoods> QueryGoodses([FromBody] PagingCriteria criteria) => Service.QueryGoodses(criteria);

    [HttpPost("[action]")]
    public Result DeleteGoodses([FromBody] List<KmGoods> models) => Service.DeleteGoodses(models);

    [HttpPost("[action]")]
    public Result SaveGoods([FromBody] object model) => Service.SaveGoods(GetDynamicModel(model));//转成dynamic类型
}
```

### 3.2. 创建Service类

- 在KIMS.Core项目Services文件夹下创建GoodsService类
- 该类为业务逻辑服务类，继承ServiceBase类

```csharp
class GoodsService : ServiceBase
{
    internal GoodsService(Context context) : base(context) { }
    //分页查询
    internal PagingResult<KmGoods> QueryGoodses(PagingCriteria criteria)
    {
        return GoodsRepository.QueryGoodses(Database, criteria);
    }
    //删除数据
    internal Result DeleteGoodses(List<KmGoods> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        //此处增加删除数据校验
        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in models)
            {
                db.Delete(item);
            }
        });
    }
    //保存数据
    internal Result SaveGoods(dynamic model)
    {
        var entity = Database.QueryById<KmGoods>((string)model.Id);
        entity ??= new KmGoods { CompNo = CurrentUser.CompNo };
        entity.FillModel(model);
        var vr = entity.Validate();
        if (vr.IsValid)
        {
            if (GoodsRepository.ExistsGoods(Database, entity))
                return Result.Error("商品编码已存在。");
        }

        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            if (entity.IsNew)
            {
                entity.Code = GetGoodsMaxNo(db);
            }
            db.Save(entity);
        }, entity.Id);
    }
    //获取商品最大编码
    private static string GetGoodsMaxNo(Database db)
    {
        var prefix = "G";
        var maxNo = GoodsRepository.GetGoodsMaxNo(db, prefix);
        if (string.IsNullOrWhiteSpace(maxNo))
            maxNo = $"{prefix}0000";
        return GetMaxFormNo(prefix, maxNo);
    }
}
```

### 3.3. 创建Repository类

- 在KIMS.Core项目Repositories文件夹下创建GoodsRepository类
- 该类为数据访问类

```csharp
class GoodsRepository
{
    //分页查询
    internal static PagingResult<KmGoods> QueryGoodses(Database db, PagingCriteria criteria)
    {
        var sql = "select * from KmGoods where CompNo=@CompNo";
        return db.QueryPage<KmGoods>(sql, criteria);//查询条件自动绑定
    }
    //获取商品最大编码
    internal static string GetGoodsMaxNo(Database db, string prefix)
    {
        var sql = $"select max(Code) from KmGoods where CompNo=@CompNo and Code like '{prefix}%'";
        return db.Scalar<string>(sql, new { db.User.CompNo });
    }
    //判断商品是否已存在
    internal static bool ExistsGoods(Database db, KmGoods entity)
    {
        var sql = "select count(*) from KmGoods where Id<>@Id and Code=@Code";
        return db.Scalar<int>(sql, new { entity.Id, entity.Code }) > 0;
    }
}
```

### 3.4. 创建Import类

- 在KIMS.Core项目Imports文件夹下创建KmGoodsImport类（约定：类名以实体类名+Import）
- 该类为数据异步导入处理类，由框架自动调用，继承BaseImport类

```csharp
class KmGoodsImport : BaseImport
{
    public KmGoodsImport(Database database) : base(database) { }
    //定义导入栏位，自动生成导入规范
    public override List<ImportColumn> Columns
    {
        get
        {
            return new List<ImportColumn>
            {
                new ImportColumn("商品类型", true),
                new ImportColumn("商品编码", true),
                new ImportColumn("商品名称", true),
                new ImportColumn("计量单位", true),
                new ImportColumn("规格型号", true),
                new ImportColumn("税率"),
                new ImportColumn("库存下限"),
                new ImportColumn("库存上限"),
                new ImportColumn("备注")
            };
        }
    }
    //异步导入处理逻辑
    public override Result Execute(SysFile file)
    {
        var models = new List<KmGoods>();
        var result = ImportHelper.ReadFile(file, row =>
        {
            var model = new KmGoods
            {
                CompNo = file.CompNo,
                Type = row.GetValue("商品类型"),
                Code = row.GetValue("商品编码"),
                Name = row.GetValue("商品名称"),
                Unit = row.GetValue("计量单位"),
                Model = row.GetValue("规格型号"),
                TaxRate = row.GetValue<decimal?>("税率"),
                MinStock = row.GetValue<decimal?>("库存下限"),
                MaxStock = row.GetValue<decimal?>("库存上限"),
                Note = row.GetValue("备注")
            };
            var vr = model.Validate();
            if (vr.IsValid)
            {
                if (models.Exists(m => m.Code == model.Code))
                    vr.AddError("商品编码不能重复！");
                else if (GoodsRepository.ExistsGoods(Database, model))
                    vr.AddError($"系统已经存在该商品编码！");
            }

            if (!vr.IsValid)
                row.ErrorMessage = vr.Message;
            else
                models.Add(model);
        });

        if (!result.IsValid)
            return result;

        return Database.Transaction("导入", db =>
        {
            foreach (var item in models)
            {
                db.Save(item);
            }
        });
    }
}
```

## 4. 运行测试

- 运行效果如下

![输入图片说明](https://foruda.gitee.com/images/1684222123055577650/f6637b2f_14334.png "屏幕截图")
![输入图片说明](https://foruda.gitee.com/images/1684222163063817213/d156efad_14334.png "屏幕截图")
