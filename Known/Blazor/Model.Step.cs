namespace Known.Blazor;

/// <summary>
/// 步骤配置模型信息类。
/// </summary>
/// <param name="component">模型关联的组件对象。</param>
public class StepModel(IBaseComponent component) : BaseModel(component)
{
    /// <summary>
    /// 取得或设置步骤CSS类名。
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// 取得或设置步骤方向是否垂直，否则水平。
    /// </summary>
    public bool IsVertical { get; set; }

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