---
name: known-framework
description: >
  Known 框架全栈开发专家。掌握 Known 框架（基于 Blazor 的插件化 C# 全栈框架）的完整开发流程：
  项目初始化与模块注册、实体模型设计（特性驱动）、三段式服务开发（接口+客户端代理+服务端实现）、
  低代码列表页与表单页开发、权限菜单配置、工作流集成、Excel 导入/导出、附件处理等。
  遇到与 Known 框架相关的开发任务时，优先调用此 Skill。
agent_created: true
---

# Known 框架开发专家指南

## 框架概述

Known 是基于 Blazor 的插件化全栈业务开发框架，使用 C# 一套代码，支持多端运行：
- **Server**：Blazor Server 模式（SSR）
- **Wasm**：WebAssembly 客户端模式
- **WinForm / MAUI / Photino**：桌面端模式

核心设计理念：**特性驱动元数据 + 约定优于配置**，通过 `[Column]`、`[Form]`、`[Menu]`、`[Action]` 等特性描述元数据，框架自动生成 UI 和权限控制。

---

## 1. 项目结构与模块注册

### 1.1 插件程序集结构（标准布局）

```
YourPlugin/
├── AppModule.cs          # 模块注册入口
├── Entities/             # 实体定义（数据库表映射）
│   └── TbXxx.cs
├── Services/             # 三段式服务
│   └── XxxService.cs
├── Pages/                # Blazor 页面
│   └── Xxx/
│       ├── XxxList.cs    # 列表页（BaseTablePage<T>）
│       └── XxxForm.razor # 表单页（BaseForm<T>）
└── WorkFlows/            # 工作流（可选）
    └── XxxFlow.cs
```

### 1.2 模块注册（AppModule.cs）

```csharp
public class AppModule
{
    // 在宿主项目 Program.cs 中调用 AppModule.AddYourModule()
    public static void AddYourModule()
    {
        // 注册程序集（框架自动扫描特性、路由、服务）
        Config.AddModule(typeof(AppModule).Assembly);

        // 添加菜单分组（导航栏顶级菜单）
        Config.Modules.AddItem("YourModuleName", "模块显示名称", "antd-icon-name", items =>
        {
            // items 由框架从 [Menu] 特性自动填充，也可手动添加
        });

        // 扩展用户信息表单 Tab（可选）
        UIConfig.UserFormTabs.Set<YourUserTab>("TabId");
    }
}
```

### 1.3 宿主项目注册

```csharp
// Program.cs 或 Startup.cs
AppModule.AddSample(); // Known.Sample 示例
AppModule.AddYourModule(); // 自定义模块
```

---

## 2. 实体定义（Entities）

### 2.1 实体基类体系

```
BaseEntity
└── EntityBase<TKey>       // 泛型主键
    └── EntityBase         // string 主键（推荐，含完整审计字段）
```

**EntityBase 自动包含的审计字段：**

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `Id` | `string` | 主键（GUID 字符串） |
| `CreateBy` | `string` | 创建人 |
| `CreateTime` | `DateTime` | 创建时间 |
| `ModifyBy` | `string` | 修改人 |
| `ModifyTime` | `DateTime?` | 修改时间 |
| `Version` | `int` | 版本号（乐观锁） |
| `Extension` | `string` | 扩展字段（JSON） |
| `AppId` | `string` | 应用 ID |
| `CompNo` | `string` | 租户企业编码 |

### 2.2 实体字段特性

```csharp
[DisplayName("显示名称")]          // 字段显示名
[Required]                          // 必填校验
[MaxLength(200)]                    // 最大长度
[Category("CategoryCode")]          // 下拉字典分类

// 表格列配置
[Column(
    IsQuery = true,         // 是否作为查询条件
    IsSort = true,          // 是否可排序
    IsViewLink = true,      // 是否渲染为查看链接
    Width = 120,            // 列宽（px）
    Fixed = "left",         // 固定列：left/right
    Align = "center",       // 对齐：left/center/right
    IsSum = true            // 是否显示合计
)]

// 表单字段配置
[Form(
    Row = 1, Column = 2,            // 所在行列（布局）
    Type = nameof(FieldType.Date),  // 字段类型
    ReadOnly = true,                // 只读
    Placeholder = "提示文字",
    FieldValue = "DefaultVal",      // 默认值
    Rows = 3,                       // 多行文本行数
    Unit = "元"                     // 字段后缀单位
)]
```

**FieldType 枚举常用值：**

```
Text / TextArea / Password
Number / Switch / CheckBox
Date / DateTime / DateRange
Select / Radio / CheckList
File / Image / Upload
RichText / Code
```

### 2.3 完整实体示例

```csharp
/// <summary>物料表</summary>
[Table("TB_Material")]
public class TbMaterial : EntityBase
{
    [DisplayName("物料编码")]
    [Required, MaxLength(50)]
    [Column(IsQuery = true, IsSort = true, Width = 120)]
    [Form(Row = 1, Column = 1)]
    public string Code { get; set; }

    [DisplayName("物料名称")]
    [Required, MaxLength(100)]
    [Column(IsQuery = true, Width = 150)]
    [Form(Row = 1, Column = 2)]
    public string Name { get; set; }

    [DisplayName("规格型号")]
    [MaxLength(200)]
    [Form(Row = 2, Column = 1)]
    public string Spec { get; set; }

    [DisplayName("单位")]
    [MaxLength(20)]
    [Category("Unit")]                // 绑定字典
    [Form(Row = 2, Column = 2)]
    public string Unit { get; set; }

    [DisplayName("库存数量")]
    [Column(IsSum = true, Align = "right")]
    [Form(Row = 3, Column = 1, Type = nameof(FieldType.Number), Unit = "件")]
    public decimal Qty { get; set; }

    [DisplayName("备注")]
    [Form(Row = 4, Column = 1, Type = nameof(FieldType.TextArea), Rows = 3)]
    public string Remark { get; set; }
}
```

---

## 3. 三段式服务（Services）

所有业务逻辑的标准结构：**接口** → **客户端代理** → **服务端实现**，三者定义在同一文件中。

### 3.1 接口定义

```csharp
// 接口继承 IService，Context 属性由框架注入
public interface IXxxService : IService
{
    // 分页查询
    Task<PagingResult<TbXxx>> QueryXxxsAsync(PagingCriteria criteria);
    // 保存（新增/编辑）
    Task<Result> SaveXxxAsync(XxxFormInfo info);
    // 批量删除
    Task<Result> DeleteXxxsAsync(List<TbXxx> models);
    // 带附件保存
    Task<Result> SaveXxxAsync(UploadInfo<TbXxx> info);
}
```

### 3.2 客户端代理（Wasm/混合模式必须）

```csharp
// [Client] 特性标记，框架自动注册为 Scoped 服务
[Client]
class XxxClient(HttpClient http) : ClientBase(http), IXxxService
{
    public Task<PagingResult<TbXxx>> QueryXxxsAsync(PagingCriteria criteria)
        => PostAsync<PagingResult<TbXxx>>("Xxx/QueryXxxs", criteria);

    public Task<Result> SaveXxxAsync(XxxFormInfo info)
        => PostAsync<Result>("Xxx/SaveXxx", info);

    public Task<Result> DeleteXxxsAsync(List<TbXxx> models)
        => PostAsync<Result>("Xxx/DeleteXxxs", models);

    // 带附件的 HTTP 上传
    public Task<Result> SaveXxxAsync(UploadInfo<TbXxx> info)
        => PostAsync<Result>("Xxx/SaveXxx", info);
}
```

**ClientBase 常用方法：**

| 方法 | 说明 |
|------|------|
| `PostAsync<T>(url, data)` | POST 请求，返回泛型结果 |
| `GetAsync<T>(url)` | GET 请求，返回泛型结果 |
| `GetFileAsync(url, fileName)` | 下载文件 |

### 3.3 服务端实现

```csharp
// [WebApi] 暴露为 API 端点
// [Service] 标记为服务端实现，自动注册为 Scoped
[WebApi, Service]
class XxxService(Context context) : ServiceBase(context), IXxxService
{
    // ↑ ServiceBase 提供：
    //   Database  - 数据库访问（自动按租户切换）
    //   CurrentUser - 当前用户
    //   Language  - 多语言
    //   App       - 应用配置

    public async Task<PagingResult<TbXxx>> QueryXxxsAsync(PagingCriteria criteria)
    {
        // 方式一：通过 ORM 直接查询
        return await Database.QueryPageAsync<TbXxx>(criteria);
        // 方式二：通过 Repository 查询（支持自定义 SQL/条件）
        // return await XxxRepository.QueryPageAsync(Database, criteria);
    }

    public async Task<Result> SaveXxxAsync(TbXxx model)
    {
        // 校验
        var result = model.Validate(Context);
        if (!result.IsValid)
            return result;

        // 保存（新增/更新 自动判断）
        await Database.SaveAsync(model);
        return Result.Success("保存成功！", model);
    }

    public async Task<Result> DeleteXxxsAsync(List<TbXxx> models)
    {
        // 单事务批量删除
        return await Database.TransactionAsync("删除", async db =>
        {
            foreach (var item in models)
                await db.DeleteAsync(item);
        });
    }

    // 带附件保存（使用 UploadInfo<T>）
    public async Task<Result> SaveXxxAsync(UploadInfo<TbXxx> info)
    {
        return await Database.TransactionAsync("保存", async db =>
        {
            var isNew = info.Model.IsNew;
            await db.SaveAsync(info.Model);

            // 处理附件（key 对应实体中 [Form(Type=File)] 字段名）
            if (isNew)
                await db.AddFilesAsync(info, "AttachField", info.Model.Id);
            else
            {
                await db.DeleteFilesAsync("AttachField", info.Model.Id);
                await db.AddFilesAsync(info, "AttachField", info.Model.Id);
            }
        });
    }
}
```

**Database 常用 ORM API：**

| 方法 | 说明 |
|------|------|
| `QueryPageAsync<T>(criteria)` | 分页查询 |
| `QueryListAsync<T>(...)` | 查询列表 |
| `QueryByIdAsync<T>(id)` | 按主键查询 |
| `QueryAsync<T>(expression)` | 条件查询单条 |
| `SaveAsync(entity)` | 新增或更新（按 Id 判断） |
| `DeleteAsync(entity)` | 删除 |
| `DeleteAsync<T>(ids)` | 按 ID 批量删除 |
| `TransactionAsync(name, action)` | 事务包装 |
| `AddFilesAsync(info, key, bizId)` | 保存附件 |
| `DeleteFilesAsync(key, bizId)` | 删除附件 |
| `CreateFlowAsync(info)` | 创建工作流实例 |
| `DeleteFlowAsync(bizId)` | 删除工作流实例 |
| `QueryActionAsync<T>(criteria)` | 查询操作日志 |

**Result 统一返回：**

```csharp
// 成功（带消息 + 可选数据）
return Result.Success("操作成功！");
return Result.Success("保存成功！", entity);

// 失败
return Result.Error("参数不正确！");
return Result.Error("错误原因", errorData);

// 校验
var result = new Result();
result.AddError("字段错误信息");
result.Required(Context, "字段名", value);   // 必填校验
result.Validate(condition, "条件不满足时的错误");
if (!result.IsValid) return result;
```

---

## 4. 列表页（Pages）

### 4.1 标准列表页

```csharp
// [Menu] 自动注册菜单和路由
// [Route] 可选，指定自定义路由
[Menu("菜单显示名称")]
[Route("/module/xxx")]
class XxxList : BaseTablePage<TbXxx>
{
    private IXxxService _service;

    // 服务注入
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        _service = await CreateServiceAsync<IXxxService>();

        // 绑定查询方法
        Table.OnQuery = QueryXxxsAsync;

        // 指定表单组件类型（自定义 Razor 表单）
        Table.FormType = typeof(XxxForm);

        // 若不指定 FormType，框架用 [Form] 特性自动生成表单
        // Table.Form.Init<TbXxx>();  // 使用 [Form] 特性生成
    }

    // 分页查询回调
    private Task<PagingResult<TbXxx>> QueryXxxsAsync(PagingCriteria criteria)
        => _service.QueryXxxsAsync(criteria);

    // [Action] 定义工具栏/行操作按钮
    [Action] public void New()         => Table.NewForm(_service.SaveXxxAsync);
    [Action] public void DeleteM()     => Table.DeleteM(_service.DeleteXxxsAsync);
    [Action] public void Edit(TbXxx row)   => Table.EditForm(_service.SaveXxxAsync, row);
    [Action] public void Delete(TbXxx row) => Table.Delete(_service.DeleteXxxsAsync, row);
    [Action] public async void Export() => await ExportDataAsync();
}
```

### 4.2 带 Tab 状态分页

```csharp
[Menu("工单管理")]
class WorkList : BaseTablePage<TbWork>
{
    private IWorkService _service;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        _service = await CreateServiceAsync<IWorkService>();

        Table.OnQuery = QueryWorksAsync;
        Table.FormType = typeof(WorkForm);

        // 按权限添加 Tab（Context.HasButton<T>(id) 内部判断）
        Table.AddTab<WorkList>(WorkStatus.Pending, "待提交");
        Table.AddTab<WorkList>(WorkStatus.Submitted, "已提交");
        Table.AddTab<WorkList>(WorkStatus.Approved, "已审批");
    }

    private Task<PagingResult<TbWork>> QueryWorksAsync(PagingCriteria criteria)
    {
        // 根据当前 Tab 动态注入查询条件
        criteria.SetQuery(nameof(TbWork.Status), QueryType.Equal, Table.CurrentTab);
        return _service.QueryWorksAsync(criteria);
    }

    // [Action(Tabs = [...])] 限定按钮仅在指定 Tab 显示
    [Action] public void New()  => Table.NewForm(_service.SaveWorkAsync);
    [Action(Tabs = [WorkStatus.Pending])] public void Submit(TbWork row)   => SubmitWork(row);
    [Action(Tabs = [WorkStatus.Submitted])] public void Approve(TbWork row) => ApproveWork(row);
    [Action] public void Edit(TbWork row)   => Table.EditForm(_service.SaveWorkAsync, row);
    [Action] public void Delete(TbWork row) => Table.Delete(_service.DeleteWorksAsync, row);
}
```

### 4.3 TableModel<T> 关键 API

```csharp
// 绑定查询
Table.OnQuery = async criteria => await _service.QueryXxxsAsync(criteria);

// 指定自定义 Razor 表单（推荐）
Table.FormType = typeof(XxxForm);

// 启用高级搜索
Table.AdvSearch = true;
// 显示分页器
Table.ShowPager = true;
// 显示列设置
Table.ShowSetting = true;
// 隐藏工具条
Table.ShowToolbar = false;

// Tab 管理
Table.AddTab("tabId");                         // 简单 Tab
Table.AddTab("tabId", "Tab 标题");
Table.AddTab<T>(id, model);                    // 嵌套 TableModel Tab
Table.CurrentTab;                              // 当前选中 Tab ID
Table.Tab.OnChangeAsync = ...;                 // Tab 切换回调

// 列操作（代码方式，配合 [Column] 特性）
Table.Column(x => x.FieldName)
    .Template((value, row) => builder => { ... });  // 自定义渲染模板

// 新增表单
Table.NewForm(onSave, defaultRow);
Table.NewForm<T>(onSave, defaultRow);
await Table.NewFormAsync(onSave, asyncRowFactory);

// 编辑表单
Table.EditForm(onSave, row);
Table.EditForm(onSave, row, actionName);

// 删除
Table.Delete(onDelete, row);    // 单条（含确认弹窗）
Table.DeleteM(onDelete);        // 批量（选中行）

// 刷新
await Table.RefreshAsync();
await Table.PageRefreshAsync();

// 额外查询字段（不在 [Column] 特性中配置）
Table.AddQueryColumn(x => x.ExtraField);
Table.AddQueryColumn("fieldId", "字段名", QueryType.Equal, "defaultValue");
```

---

## 5. 表单页（Forms）

### 5.1 Razor 表单（推荐用于复杂表单）

**XxxForm.razor：**

```razor
@* 继承 BaseForm<TEntity>，T 为实体类型 *@
@inherits BaseForm<TbXxx>

<DynamicForm Model="Model" />

@* 含附件 *@
<DynamicForm Model="Model" />
<KUpload Model="Model" Field="@nameof(TbXxx.AttachFile)" />

@* 含流程日志 *@
<FlowLogGrid BizId="@Model.Data?.Id" />
```

**XxxForm.razor.cs：**

```csharp
partial class XxxForm : BaseForm<TbXxx>
{
    private IXxxService _service;

    // 初始化（OnInitFormAsync 在 SetParametersAsync 之后调用）
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        _service = await CreateServiceAsync<IXxxService>();

        // 绑定保存（普通）
        Model.OnSave = _service.SaveXxxAsync;
        // 绑定保存（带附件）
        Model.OnSaveFile = _service.SaveXxxAsync;
        // 保存前回调（返回 false 阻止保存）
        Model.OnSaving = async item =>
        {
            // 业务校验
            if (!CheckValid(item))
                return false;
            return true;
        };
        // 保存后回调
        Model.OnSaved = item => { /* 刷新关联数据等 */ };
    }

    // 首次渲染时异步加载数据
    protected override async Task OnRenderAsync(bool firstRender)
    {
        if (firstRender && !Model.IsNew)
        {
            // 加载关联数据
            var detail = await _service.GetXxxDetailAsync(Model.Data.Id);
            // 更新 UI
            await StateChangedAsync();
        }
    }
}
```

### 5.2 FormModel<T> 关键 API

```csharp
// 状态判断
Model.IsNew     // 是否新增模式
Model.IsView    // 是否查看模式（只读）
Model.Data      // 当前表单数据（TItem 实例）

// 布局
Model.Draggable   // 对话框可拖动，默认 true
Model.Resizable   // 对话框可调整大小

// 自定义内容插槽
Model.Header      // 对话框头部 RenderFragment
Model.Footer      // 对话框底部 RenderFragment
Model.FooterLeft  // 底部左侧 RenderFragment
Model.FooterRight // 底部右侧（按钮左侧）RenderFragment

// 附件
Model.Files       // Dictionary<string, List<FileDataInfo>>
Model.HasFile(key) // 是否有附件

// 保存钩子
Model.OnSave      // Func<TItem, Task<Result>>
Model.OnSaveFile  // Func<UploadInfo<TItem>, Task<Result>>
Model.OnSaving    // Func<TItem, Task<bool>>（返回 false 中止保存）
Model.OnSaved     // Action<TItem>（同步，保存后）
Model.OnSavedAsync // Func<TItem, Task>（异步，保存后）
Model.ConfirmText  // 保存前确认弹窗文字

// 触发保存
await Model.SaveAsync();            // 保存并关闭
await Model.SaveAsync(false);       // 保存不关闭
await Model.SaveContinueAsync();    // 保存并继续新增
```

### 5.3 多泛型表单（含扩展信息）

```csharp
// 当表单数据结构超出单一实体时，使用扩展 DTO
@inherits BaseForm<TbMaterial, PackFieldInfo>
// TbMaterial 是主实体，PackFieldInfo 是扩展字段 DTO
```

### 5.4 Tab 表单（BaseTabForm）

```csharp
// 多 Tab 布局的复杂表单
class XxxForm : BaseTabForm
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Tab.AddTab("Basic", "基本信息", b => b.Component<XxxBasicTab>().Build());
        Tab.AddTab("Detail", "详情信息", b => b.Component<XxxDetailTab>().Build());
    }
}
```

### 5.5 步骤表单（BaseStepForm）

```csharp
class XxxForm : BaseStepForm
{
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Step.AddStep("step1", "填写基本信息");
        Step.AddStep("step2", "填写详细信息");
        Step.AddStep("step3", "确认提交");
    }
}
```

---

## 6. 权限与菜单

### 6.1 菜单特性

```csharp
// 自动注册路由和菜单
[Menu("菜单名称")]
[Menu("菜单名称", Icon = "icon-name", Sort = 10, ParentId = "ParentMenuId")]
[Route("/module/xxx")]

// 角色权限（Role 特性，指定允许访问的角色）
[Role("admin", "manager")]

// Tab 角色权限（不同 Tab 需要不同角色）
[TabRole(Tab = "Approved", Role = "approver")]
```

### 6.2 操作按钮特性

```csharp
// 基本用法（方法名即为按钮 ID）
[Action]
public void New() => ...

// 完整配置
[Action(
    Icon = "plus",          // 图标（Ant Design 图标名）
    Name = "新增",           // 显示名称（默认用语言包翻译方法名）
    Style = "primary",      // 样式：primary / danger / default
    Visible = true,         // 是否可见
    Group = "export",       // 按钮分组（折叠菜单）
    Tabs = ["tab1", "tab2"] // 仅在指定 Tab 下显示
)]
public void New() => ...
```

---

## 7. 工作流集成

### 7.1 工作流定义（FlowBase）

```csharp
// 继承 FlowBase，定义业务流程步骤
class XxxFlow : FlowBase
{
    // 流程步骤定义在这里
    // 通常包含：提交、审批、驳回、撤回等节点
}
```

### 7.2 工作流初始化（服务端）

```csharp
public async Task<Result> SaveXxxAsync(UploadInfo<TbWork> info)
{
    return await Database.TransactionAsync("保存工单", async db =>
    {
        var isNew = info.Model.IsNew;
        await db.SaveAsync(info.Model);

        if (isNew)
        {
            // 创建工作流实例
            var bizInfo = new FlowBizInfo
            {
                BizId   = info.Model.Id,
                BizName = info.Model.Name,     // 业务名称（显示在流程记录中）
                BizUrl  = "/module/work",       // 业务跳转 URL
                FlowCode = "XxxFlowCode",       // 流程编码（在系统配置中预先定义）
                User     = CurrentUser
            };
            await db.CreateFlowAsync(bizInfo);
            await db.AddFilesAsync(info, nameof(TbWork.AttachFile), info.Model.Id);
        }
    });
}
```

### 7.3 在表单中显示流程记录

```razor
@inherits BaseForm<TbWork>

<DynamicForm Model="Model" />
<KUpload Model="Model" Field="@nameof(TbWork.AttachFile)" />

@* 流程记录（需要有 BizId） *@
<FlowLogGrid BizId="@Model.Data?.Id" />
```

---

## 8. Excel 导入（Import）

### 8.1 导入类定义

```csharp
// [Import] 特性绑定实体类型
[Import(typeof(TbXxx))]
class XxxImport : ImportBase<TbXxx>
{
    // 定义 Excel 列映射
    protected override void InitColumns()
    {
        // AddColumn(字段名, Excel列名, 是否必填)
        AddColumn(nameof(TbXxx.Code), "物料编码", true);
        AddColumn(nameof(TbXxx.Name), "物料名称", true);
        AddColumn(nameof(TbXxx.Spec), "规格型号");
        AddColumn(nameof(TbXxx.Unit), "单位");
        AddColumn(nameof(TbXxx.Qty),  "数量");
    }

    // 可选：覆盖导入逻辑（每行数据处理）
    public override async Task<Result> ExecuteAsync(Database db, List<TbXxx> models)
    {
        return await db.TransactionAsync("导入", async transaction =>
        {
            foreach (var model in models)
            {
                model.CompNo = CurrentUser.CompNo;
                await transaction.SaveAsync(model);
            }
        });
    }
}
```

### 8.2 列表页触发导入

```csharp
// 在 XxxList.cs 中添加导入按钮
[Action] public async void Import() => await Table.ShowImportAsync();
```

---

## 9. Excel 导出

### 9.1 在服务端实现导出

```csharp
// 接口
Task<byte[]> ExportXxxsAsync(PagingCriteria criteria);

// 客户端
public Task<byte[]> ExportXxxsAsync(PagingCriteria criteria)
    => PostAsync<byte[]>("Xxx/ExportXxxs", criteria);

// 服务端
public async Task<byte[]> ExportXxxsAsync(PagingCriteria criteria)
{
    // QueryPageAsync 不带分页限制时返回所有数据
    criteria.PageSize = 0;
    var result = await Database.QueryPageAsync<TbXxx>(criteria);
    // 使用框架内置 Excel 导出
    return ExcelHelper.ExportData(result.PageData, GetColumns<TbXxx>());
}
```

### 9.2 在列表页触发导出

```csharp
// 方式一：使用框架默认导出（推荐）
[Action] public async void Export() => await ExportDataAsync();

// 方式二：自定义导出
[Action]
public async void Export()
{
    var data = await _service.ExportXxxsAsync(Table.Criteria);
    await JS.DownloadFileAsync("物料数据.xlsx", data);
}
```

---

## 10. 附件处理

### 10.1 实体中定义附件字段

```csharp
[DisplayName("附件")]
[Form(Type = nameof(FieldType.File))]
public string AttachFile { get; set; }
```

### 10.2 服务端附件操作

```csharp
// 保存附件（保存实体后调用）
await db.AddFilesAsync(uploadInfo, fieldName, entityId);

// 删除附件（更新时先删再加）
await db.DeleteFilesAsync(fieldName, entityId);

// 删除实体时同步清理附件
AttachFile.DeleteFiles(entity.AttachFile);
```

### 10.3 表单中展示上传组件

```razor
@* KUpload 组件自动处理上传/预览 *@
<KUpload Model="Model" Field="@nameof(TbXxx.AttachFile)" />

@* 图片上传 *@
<KUpload Model="Model" Field="@nameof(TbXxx.Avatar)" 
         Accept="image/*" MaxCount="1" />
```

---

## 11. UIConfig 全局配置

```csharp
// 扩展用户表单 Tab
UIConfig.UserFormTabs.Set<YourUserTab>("tabId");

// 自定义登录页
UIConfig.LoginPage = typeof(CustomLoginPage);

// 全局设置导航菜单位置
UIConfig.NavPosition = NavPosition.Left;
```

---

## 12. Context 对象

`Context` 在 `ServiceBase` 和 `BasePage` 中均可访问：

```csharp
// 当前用户
var user = Context.CurrentUser;
user.UserName    // 用户名
user.Name        // 显示名
user.CompNo      // 租户编码
user.Role        // 角色

// 多语言
var text = Context.Language["Key"];
var title = Context.Language.GetFormTitle(action, name);

// 移动端判断
bool isMobile = Context.IsMobile;

// 获取服务
var service = Context.GetService<IXxxService>();

// 获取参数
var param = Context.GetParameter<string>("key");
```

---

## 13. 开发快速检查清单

开始开发新功能时，按以下顺序检查：

```
□ 1. 实体（Entities/TbXxx.cs）
      - 继承 EntityBase
      - 每个字段添加 [DisplayName]、[Column]、[Form] 特性
      - 必填字段添加 [Required]
      - 字典字段添加 [Category("Code")]

□ 2. 服务接口（IXxxService）
      - 继承 IService
      - 定义 QueryXxxsAsync / SaveXxxAsync / DeleteXxxsAsync

□ 3. 客户端代理（XxxClient）
      - [Client] 特性
      - 继承 ClientBase(http)
      - 实现接口所有方法，调用 PostAsync

□ 4. 服务端实现（XxxService）
      - [WebApi, Service] 特性
      - 继承 ServiceBase(context)
      - QueryPageAsync / SaveAsync / TransactionAsync

□ 5. 列表页（XxxList.cs）
      - [Menu] + [Route] 特性
      - 继承 BaseTablePage<TbXxx>
      - OnInitPageAsync 中绑定 OnQuery、FormType
      - [Action] 定义 New、Edit、Delete、DeleteM

□ 6. 表单页（XxxForm.razor + .cs）
      - @inherits BaseForm<TbXxx>
      - OnInitFormAsync 中绑定 OnSave/OnSaveFile
      - 复杂逻辑在 OnRenderAsync(firstRender) 中加载

□ 7. 模块注册（AppModule.cs）
      - Config.AddModule(assembly)
      - Config.Modules.AddItem(...)

□ 8. （可选）导入（XxxImport.cs）
      - [Import(typeof(TbXxx))]
      - 继承 ImportBase<TbXxx>
      - InitColumns() 定义列映射

□ 9. （可选）工作流（XxxFlow.cs）
      - 继承 FlowBase
      - 服务端 CreateFlowAsync / DeleteFlowAsync
      - 表单 <FlowLogGrid BizId="..." />
```

---

## 14. 常见问题与注意事项

1. **`[Client]` 和 `[Service]` 类必须是 `internal class`，不能是 `public`**，接口是 `public interface`。

2. **三段式中客户端 URL 必须与服务端路由匹配**：`[WebApi]` 会自动注册路由，格式为 `{ControllerName}/{MethodName}`，其中 ControllerName 来自类名去掉 `Service` 后缀（如 `XxxService` → `Xxx`）。

3. **`Database.SaveAsync` 自动判断新增/更新**：通过 `entity.Id` 是否为空或 `entity.IsNew` 来区分。如果是新增，`Id` 会自动生成 GUID。

4. **`Result.IsValid` vs `Result.IsOk`**：`IsValid` 用于服务端校验（`AddError` 后为 false）；`IsOk` 用于 HTTP 响应成功。

5. **附件字段在实体中只存元数据**（文件路径或引用 ID），实际文件通过 `AddFilesAsync` 存储在文件服务中。

6. **多租户支持**：`Database` 属性会根据 `CurrentUser.CompNo` 自动切换数据库连接，无需手动处理。

7. **`Table.OnQuery` 中的 `PagingCriteria`**：包含分页参数（`PageIndex`、`PageSize`）和查询参数（`Query` 字典），`criteria.SetQuery(field, type, value)` 用于注入动态条件。

8. **Tab 列表页刷新**：切换 Tab 时框架自动调用 `Table.OnQuery`，无需手动触发。

9. **`[Action]` 方法签名规则**：
   - 无参数 → 工具栏按钮（批量操作）
   - 参数为 `TItem` → 行操作按钮

10. **表单中禁止直接修改 `Model.Data`**，应通过双向绑定 `@bind-Value` 让 Blazor 自动同步，或在 `OnSaving` 回调中修改。
