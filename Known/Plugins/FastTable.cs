namespace Known.Plugins;

/// <summary>
/// 快速添加表格接口。
/// </summary>
/// <typeparam name="TItem">添加数据类型。</typeparam>
public interface IFastTable<TItem> : Microsoft.AspNetCore.Components.IComponent
{
    /// <summary>
    /// 取得或设置查询参数。
    /// </summary>
    string Query { get; set; }

    /// <summary>
    /// 取得或设置已选择的数据列表。
    /// </summary>
    List<string> SelectedItems { get; set; }

    /// <summary>
    /// 取得选择的数据列表。
    /// </summary>
    IEnumerable<TItem> SelectedRows { get; }

    /// <summary>
    /// 取得或设置表格行双击事件委托。
    /// </summary>
    Func<TItem, Task> OnRowDoubleClick { get; set; }
}