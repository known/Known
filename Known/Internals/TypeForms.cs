using AntDesign;

namespace Known.Internals;

/// <summary>
/// 系统设置表单组件类。
/// </summary>
public class SettingTypeForm : Form<UserSettingInfo> { }

/// <summary>
/// 查询条件表单组件类。
/// </summary>
public class QueryDataForm : Form<Dictionary<string, QueryInfo>> { }