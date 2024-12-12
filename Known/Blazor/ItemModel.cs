namespace Known.Blazor;

/// <summary>
/// 列表项模型信息类。
/// </summary>
/// <param name="id">列表项ID。</param>
/// <param name="title">列表项标题。</param>
public class ItemModel(string id, string title)
{
    /// <summary>
    /// 取得列表项ID。
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// 取得列表项标题。
    /// </summary>
    public string Title { get; } = title;

    /// <summary>
    /// 取得或设置列表项子标题。
    /// </summary>
    public string SubTitle { get; set; }

    /// <summary>
    /// 取得或设置列表项描述信息。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置项目是否显示。
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 取得或设置列表项呈现模板。
    /// </summary>
    public RenderFragment Content { get; set; }

    /// <summary>
    /// 取得或设置列表项表单配置模型。
    /// </summary>
    public TableModel Table { get; set; }
}

/// <summary>
/// 标签配置模型信息类。
/// </summary>
public class TabModel
{
    /// <summary>
    /// 取得或设置步骤CSS类名。
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// 取得或设置当前标签。
    /// </summary>
    public string Current { get; set; }

    /// <summary>
    /// 取得或设置标签左侧模板。
    /// </summary>
    public RenderFragment Left { get; set; }

    /// <summary>
    /// 取得或设置标签右侧模板。
    /// </summary>
    public RenderFragment Right { get; set; }

    /// <summary>
    /// 取得或设置标签项目信息列表。
    /// </summary>
    public List<ItemModel> Items { get; } = [];

    /// <summary>
    /// 取得或设置标签改变委托方法。
    /// </summary>
    internal Action<string> OnChange { get; set; }

    /// <summary>
    /// 取得或设置组件状态改变方法委托。
    /// </summary>
    internal Action OnStateChanged { get; set; }

    /// <summary>
    /// 取得标签是否有项目。
    /// </summary>
    public bool HasItem => Items != null && Items.Count > 0;

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    public void AddTab(string id) => AddTab(id, id);

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="title">标签标题。</param>
    public void AddTab(string id, string title) => Items.Add(new ItemModel(id, title));

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="title">标签标题。</param>
    /// <param name="table">标签表格配置模型。</param>
    public void AddTab(string id, string title, TableModel table) => Items.Add(new ItemModel(id, title) { Table = table });

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="content">标签呈现模板。</param>
    public void AddTab(string id, RenderFragment content) => AddTab(id, id, content);

    /// <summary>
    /// 添加一个标签。
    /// </summary>
    /// <param name="id">标签ID。</param>
    /// <param name="title">标签标题。</param>
    /// <param name="content">标签呈现模板。</param>
    public void AddTab(string id, string title, RenderFragment content) => Items.Add(new ItemModel(id, title) { Content = content });

    /// <summary>
    /// 标签切换事件。
    /// </summary>
    public void Change() => OnChange?.Invoke(Current);

    /// <summary>
    /// 改变组件状态。
    /// </summary>
    public void StateChanged() => OnStateChanged?.Invoke();
}

/// <summary>
/// 步骤配置模型信息类。
/// </summary>
public class StepModel
{
    /// <summary>
    /// 取得或设置步骤CSS类名。
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// 取得或设置步骤方向（横向/竖向）。
    /// </summary>
    public string Direction { get; set; }

    /// <summary>
    /// 取得或设置步骤当前位置。
    /// </summary>
    public int Current { get; set; }

    /// <summary>
    /// 取得或设置步骤项目信息列表。
    /// </summary>
    public List<ItemModel> Items { get; } = [];

    /// <summary>
    /// 添加一个步骤。
    /// </summary>
    /// <param name="title">步骤标题。</param>
    public void AddStep(string title) => Items.Add(new ItemModel("", title));

    /// <summary>
    /// 添加一个步骤。
    /// </summary>
    /// <param name="id">步骤ID或标题。</param>
    /// <param name="content">步骤呈现模板。</param>
    public void AddStep(string id, RenderFragment content) => AddStep(id, id, content);

    /// <summary>
    /// 添加一个步骤。
    /// </summary>
    /// <param name="id">步骤ID。</param>
    /// <param name="title">步骤标题。</param>
    /// <param name="content">步骤呈现模板。</param>
    public void AddStep(string id, string title, RenderFragment content)
    {
        Items.Add(new ItemModel(id, title) { Content = content });
    }
}

/// <summary>
/// 工具条配置模型信息类。
/// </summary>
public class ToolbarModel
{
    /// <summary>
    /// 取得或设置工具条最大显示按钮数量，默认4个。
    /// </summary>
    public int ShowCount { get; set; } = 4;

    /// <summary>
    /// 取得或设置工具条按钮信息列表。
    /// </summary>
    public List<ActionInfo> Items { get; set; } = [];

    /// <summary>
    /// 取得或设置工具条按钮单击事件委托方法。
    /// </summary>
    public Action<ActionInfo> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置工具条刷新委托。
    /// </summary>
    internal Action OnRefresh { get; set; }

    /// <summary>
    /// 取得工具条是否有项目。
    /// </summary>
    public bool HasItem => Items != null && Items.Count > 0;

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="idOrName">按钮ID或名称。</param>
    public void AddAction(string idOrName) => Items.Add(new ActionInfo(idOrName));

    /// <summary>
    /// 添加一个操作按钮。
    /// </summary>
    /// <param name="id">按钮ID。</param>
    /// <param name="name">按钮名称。</param>
    /// <param name="icon">按钮图标。</param>
    public void AddAction(string id, string name, string icon)
    {
        Items.Add(new ActionInfo
        {
            Id = id,
            Name = name,
            Icon = icon
        });
    }

    /// <summary>
    /// 刷新工具条。
    /// </summary>
    public void Refresh() => OnRefresh?.Invoke();
}