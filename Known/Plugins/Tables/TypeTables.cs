namespace Known.Plugins.Tables;

/// <summary>
/// 无代码表格设置表单组件类。
/// </summary>
public class AutoTableTypeForm : AntForm<AutoPageInfo> { }

/// <summary>
/// 无代码表格字段表格组件类。
/// </summary>
public class ActionTable : AntTable<ActionInfo> { }

/// <summary>
/// 示例表格页面组件类。
/// </summary>
public class DemoTablePage : TablePage<Dictionary<string, object>> { }

/// <summary>
/// 示例表格表单组件类。
/// </summary>
public class DemoTableForm : DataForm<Dictionary<string, object>> { }