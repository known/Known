namespace Known.Blazor;

/// <summary>
/// 页面配置模型信息类。
/// </summary>
public class PageModel
{
    /// <summary>
    /// 取得或设置页面布局类型。
    /// </summary>
	public PageType Type { get; set; }

    /// <summary>
    /// 取得或设置页面布局大小，如28，表示两列分别为20%和80%。
    /// </summary>
    public string Spans { get; set; }

    /// <summary>
    /// 取得或设置页面配置信息列表。
    /// </summary>
    public List<PageItemModel> Items { get; } = [];

    /// <summary>
    /// 取得或设置页面状态改变委托。
    /// </summary>
	public Action StateChanged { get; set; }

    /// <summary>
    /// 添加页面项目。
    /// </summary>
    /// <param name="content">页面项目呈现模板。</param>
    public void AddItem(RenderFragment content)
    {
        Items.Add(new PageItemModel { Content = content });
    }

    /// <summary>
    /// 添加页面项目。
    /// </summary>
    /// <param name="className">页面项目CSS类名。</param>
    /// <param name="content">页面项目呈现模板。</param>
    public void AddItem(string className, RenderFragment content)
    {
        Items.Add(new PageItemModel { ClassName = className, Content = content });
    }
}

/// <summary>
/// 页面项目配置模型信息类。
/// </summary>
public class PageItemModel
{
    /// <summary>
    /// 取得或设置页面项目CSS类名。
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 取得或设置页面项目呈现模板。
    /// </summary>
    public RenderFragment Content { get; set; }
}