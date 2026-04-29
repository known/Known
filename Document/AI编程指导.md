# Known 框架 AI 编程指导文档

> 面向：AI 编程助手（Claude/GPT/DeepSeek 等）
>
> 目标：帮助 AI 理解 Known 框架架构，快速生成符合框架规范的代码。

---

## 一、框架核心概念

### 1.1 整体架构

Known 是一个基于 Blazor 的插件化业务开发框架，采用「约定优于配置」的设计理念。

```
Known (核心类库)
├── 实体模型 (EntityBase)
├── 服务模型 (ServiceBase, ClientBase)
├── 数据库访问 (Database)
├── UI 组件 (Apps 命名空间)
└── 配置体系 (Config)

Plugins/ (业务插件)
├── Entities/ (业务实体)
├── Services/ (业务服务)
├── Pages/ (业务页面)
└── wwwroot/ (静态资源)
```

### 1.2 运行模式

- **Server 模式**：Blazor Server，实时交互
- **Wasm 模式**：Blazor WebAssembly，离线优先
- **WinForm/MAUI**：桌面/移动端

---

## 二、实体模型

### 2.1 基类选择

```csharp
// 方式一：使用字符串主键（推荐）
public class YourEntity : EntityBase
{
    public string Name { get; set; }
    public string Code { get; set; }
    public int Status { get; set; }
}

// 方式二：使用自定义主键类型
public class YourEntity : EntityBase<int>
{
    public string Name { get; set; }
}
```

### 2.2 字段特性

```csharp
public class YourEntity : EntityBase
{
    // 表格列元数据
    [Column("名称", IsSort = true, IsQuery = true)]
    public string Name { get; set; }

    // 表单字段元数据
    [Form(Row = 1, Column = 1, Type = "Text")]
    public string Code { get; set; }
}
```

### 2.3 常用字段特性说明

| 特性 | 属性 | 说明 |
|------|------|------|
| [Column] | Field | 数据库字段名 |
| [Column] | IsVisible | 是否可见 |
| [Column] | IsSort | 是否可排序 |
| [Column] | IsQuery | 是否可查询 |
| [Column] | IsSum | 是否汇总 |
| [Column] | Width | 列宽度 |
| [Form] | Row/Column | 表单布局行列 |
| [Form] | Type | 组件类型(Text/Select/Date/Upload等) |
| [Form] | ReadOnly | 是否只读 |

---

## 三、服务模型

### 3.1 三段式服务开发

```csharp
// 1. 定义服务接口
public interface IYourService : IService
{
    Task<List<YourEntity>> QueryAllAsync();
    Task<YourEntity> GetByIdAsync(string id);
    Task<Result> SaveAsync(YourEntity entity);
    Task<Result> DeleteAsync(string id);
}

// 2. 客户端实现（用于 Wasm/Server 调用）
[Client]
public class YourClient : ClientBase, IYourService
{
    public YourClient(HttpClient http) : base(http) { }

    public Task<List<YourEntity>> QueryAllAsync() => ...
    public Task<YourEntity> GetByIdAsync(string id) => ...
    public Task<Result> SaveAsync(YourEntity entity) => ...
    public Task<Result> DeleteAsync(string id) => ...
}

// 3. 服务端实现（业务逻辑）
[WebApi, Service]
public class YourService : ServiceBase, IYourService
{
    public YourService(Context context) : base(context) { }

    public async Task<List<YourEntity>> QueryAllAsync()
    {
        return await Database.QueryListAsync<YourEntity>();
    }

    public async Task<YourEntity> GetByIdAsync(string id)
    {
        return await Database.QueryByIdAsync<YourEntity>(id);
    }

    public async Task<Result> SaveAsync(YourEntity entity)
    {
        if (entity.IsNew)
        {
            entity.CreateBy = CurrentUser.Name;
            entity.CreateTime = DateTime.Now;
        }
        else
        {
            entity.ModifyBy = CurrentUser.Name;
            entity.ModifyTime = DateTime.Now;
        }
        await Database.SaveAsync(entity);
        return Result.Success("");
    }

    public async Task<Result> DeleteAsync(string id)
    {
        await Database.DeleteAsync<YourEntity>(id);
        return Result.Success("");
    }
}
```

### 3.2 常用特性说明

| 特性 | 目标 | 说明 |
|------|------|------|
| [Service] | Class | 注册为 DI 服务 |
| [Client] | Class | 注册为客户端代理 |
| [WebApi] | Class | 自动生成 WebAPI |
| [Anonymous] | Class/Method | 允许匿名访问 |
| [Action] | Method | 页面操作权限 |

---

## 四、数据库访问

### 4.1 Database 常用方法

```csharp
// 查询
var list = await Database.QueryListAsync<YourEntity>();
var entity = await Database.QueryByIdAsync<YourEntity>(id);
var paged = await Database.QueryPageAsync<YourEntity>(criteria);

// 保存
await Database.SaveAsync(entity);  // 自动判断新增/更新

// 删除
await Database.DeleteAsync<YourEntity>(id);
await Database.DeleteAsync<YourEntity>(new[] { "id1", "id2" });

// 执行 SQL
await Database.ExecuteAsync(sql, parameters);

// 事务
await Database.TransactionAsync(async () =>
{
    await Database.SaveAsync(entity1);
    await Database.SaveAsync(entity2);
});
```

### 4.2 分页查询条件

```csharp
var criteria = new PagingCriteria
{
    PageIndex = 1,
    PageSize = 10,
    OrderBy = "CreateTime desc"
};
criteria.Query.Add(new QueryInfo("Name", QueryType.Contain, keyword));

var paged = await Database.QueryPageAsync<YourEntity>(criteria);
```

### 4.3 查询类型

| 查询类型 | 说明 | 示例值 |
|----------|------|-------|
| Equal | 等于 | Name=张三 |
| Contain | 包含 | Name=张 |
| StartWith | 开头 | Name=张 |
| EndWith | 结尾 | Name=三 |
| Between | 范围 | CreateTime=2024-01-01~2024-12-31 |
| In | 批量 | Id in (1,2,3) |

---

## 五、页面开发

### 5.1 页面基类

```csharp
// 列表页面
public class YourList : BaseTablePage<YourEntity>

// 表单页面
public class YourForm : BaseForm<YourEntity>
```

### 5.2 列表页模式

```csharp
[Route("/your/list")]
[Menu("Parent", "你的菜单", "icon", 1)]
public class YourList : BaseTablePage<YourEntity>
{
    protected override async Task OnInitPageAsync()
    {
        // 初始化表格
        Table = new Table<YourEntity>
        {
            Columns = new List<ColumnInfo>
            {
                new ColumnInfo(nameof(YourEntity.Name)) { Title = "名称" },
                new ColumnInfo(nameof(YourEntity.Code)) { Title = "编码" },
            }
        };

        // 绑定查询
        Table.OnQuery = async p => await YourService.QueryAllAsync();

        // 设置操作
        Table.OnAdd = () => new YourEntity();
    }

    [Action(Name = "新增", Icon = "Plus")]
    public async Task OnAddAsync()
    {
        // 跳转表单页
        await Table.OpenFormAsync(new YourEntity());
    }

    [Action(Name = "编辑")]
    public async Task OnEditAsync(YourEntity row)
    {
        await Table.OpenFormAsync(row);
    }

    [Action(Name = "删除", Style = "danger")]
    public async Task OnDeleteAsync(YourEntity row)
    {
        if (await Table.ConfirmAsync("确认删除?"))
        {
            await YourService.DeleteAsync(row.Id);
            await Table.RefreshAsync();
        }
    }
}
```

### 5.3 表单页模式

```csharp
[Route("/your/form")]
public class YourForm : BaseForm<YourEntity>
{
    protected override async Task OnInitFormAsync()
    {
        Form = new FormModel<YourEntity>
        {
            OnOk = async () => await SaveAsync()
        };

        // 定义表单项
        Form.Fields = new List<FieldInfo>
        {
            new FieldInfo(nameof(YourEntity.Name)) { Label = "名称", Type = "Text", Required = true },
            new FieldInfo(nameof(YourEntity.Code)) { Label = "编码", Type = "Text", Required = true },
            new FieldInfo(nameof(YourEntity.Status)) { Label = "状态", Type = "Switch" },
        };
    }

    private async Task SaveAsync()
    {
        var result = YourService.SaveAsync(Form.Entity);
        if (result.Success)
        {
            await Form.SuccessAsync();
        }
    }
}
```

---

## 六、常用工具类

### 6.1 Utils

```csharp
// ID 生成
var id = Utils.GetNextId<string>();
var guid = Utils.GetGuid();

// 类型转换
var name = Utils.ConvertTo<string>(value);
var entity = Utils.MapTo<YourEntity>(source);

// JSON
var json = Utils.ToJson(entity);
var entity = Utils.FromJson<YourEntity>(json);

// 文件
var content = Utils.ReadFile(path);
Utils.SaveFile(path, content);
```

### 6.2 Result

```csharp
// 成功
return Result.Success("操作成功");

// 失败
return Result.Error("操作失败");

// 带数据
return Result.Success(data, "操作成功");

// 分页
return Result.Success(list, total, "查询成功");
```

### 6.3 Config

```csharp
// 读取配置
var appId = Config.App.Id;
var version = Config.Version;

// 服务获取
var service = Config.CreateService<IYourService>();
```

---

## 七、代码模板速查

### 7.1 实体模板

```csharp
[Table("YourTable")]
public class YourEntity : EntityBase
{
    [Form(FieldValue = "Text")]
    public string Name { get; set; }

    [Column(IsQuery = true)]
    public string Code { get; set; }

    public int Status { get; set; }
}
```

### 7.2 服务接口模板

```csharp
public interface IYourService : IService
{
    Task<List<YourEntity>> QueryAllAsync();
    Task<YourEntity> GetByIdAsync(string id);
    Task<Result> SaveAsync(YourEntity entity);
    Task<Result> DeleteAsync(string id);
}
```

### 7.3 页面路由写法

```csharp
[Route("/module/list")]
[Menu("Parent", "菜单名称", "Menu", 1)]
public class YourList : BaseTablePage<YourEntity>
{
    // ...
}
```

---

## 八、注意事项

1. **一个业务能力 = 实体 + 服务 + 页面 + 菜单元数据**
2. 服务接口优先，客户端/服务端都实现接口
3. 写操作使用 `TransactionAsync` 包裹
4. 统一使用 `Result` 返回业务结果
5. 公共文案放多语言，不写死
6. 字段特性优先，减少代码量

---

## 九、源码索引

| 类别 | 文件 |
|------|------|
| 实体基类 | `Known/Entity.cs` |
| 服务基类 | `Known/Service.cs` |
| 数据库访问 | `Known/Data/Database.cs` |
| 配置 | `Known/Config.cs` |
| 工具类 | `Known/Utils.cs` |
| 上下文 | `Known/Context.cs` |
| 特性定义 | `Known/Attributes.cs` |
| 服务示例 | `Plugins/Known.Sample/Services/*Service.cs` |
| 页面示例 | `Plugins/Known.Sample/Pages/*` |

---

如需具体业务模块代码生成，请提供需求描述。