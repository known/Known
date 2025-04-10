namespace Known.Internals;

/// <summary>
/// 移动端查询表单组件类。
/// </summary>
public partial class AppQueryForm
{
    /// <summary>
    /// 取得或设置表格查询数据信息字典。
    /// </summary>
    [Parameter] public Dictionary<string, QueryInfo> Data { get; set; }
}