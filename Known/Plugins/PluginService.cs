namespace Known.Plugins;

/// <summary>
/// 插件服务接口。
/// </summary>
public interface IPluginService
{
    /// <summary>
    /// 获取低代码表格页面插件设置按钮列表。
    /// </summary>
    /// <param name="page">表格页面。</param>
    /// <returns>下拉操作列表。</returns>
    List<ActionInfo> GetTableActions(BaseTablePage page);

    /// <summary>
    /// 获取低代码表单插件设置按钮列表。
    /// </summary>
    /// <param name="form">表单页面。</param>
    /// <returns>下拉操作列表。</returns>
    List<ActionInfo> GetFormActions(BaseForm form);

    /// <summary>
    /// 
    /// </summary>
    void ConfigTable();

    /// <summary>
    /// 
    /// </summary>
    void AddTableColumn();

    /// <summary>
    /// 
    /// </summary>
    void AddTableAction();

    /// <summary>
    /// 编辑工具条。
    /// </summary>
    /// <param name="toolbar">工具条。</param>
    void EditToolbar(KToolbar toolbar);
}

class PluginService : IPluginService
{
    public List<ActionInfo> GetTableActions(BaseTablePage page) => [];

    public List<ActionInfo> GetFormActions(BaseForm form) => [];

    public void ConfigTable() { }

    public void AddTableColumn() { }

    public void AddTableAction() { }

    public void EditToolbar(KToolbar toolbar) { }
}