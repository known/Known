namespace Known.Components;

/// <summary>
/// 左侧列表和右侧表格关联组件类。
/// </summary>
/// <typeparam name="TItem">表格数据类型。</typeparam>
public class KListTable<TItem> : KListPanel where TItem : class, new()
{
    /// <summary>
    /// 构造函数，初始化组件实例。
    /// </summary>
    public KListTable()
    {
        ChildContent = builder => builder.TablePage(Table);
    }

    /// <summary>
    /// 取得或设置表格配置模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Table { get; set; }
}