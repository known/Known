namespace Known.Blazor;

/// <summary>
/// UI上下文级联值组件类。
/// </summary>
public class UIContextValue : CascadingValue<UIContext> { }

/// <summary>
/// 表单数据字段项目级联值类。
/// </summary>
public class DataItemValue : CascadingValue<DataItem> { }

/// <summary>
/// 布局级联值类。
/// </summary>
public class LayoutValue : CascadingValue<BaseLayout> { }