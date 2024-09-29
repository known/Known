namespace Known.Blazor;

/// <summary>
/// 自定义表单字段组件接口。
/// </summary>
public interface ICustomField
{
    /// <summary>
    /// 取得或设置字段组件是否只读。
    /// </summary>
    bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置字段组件绑定的值。
    /// </summary>
    object Value { get; set; }

    /// <summary>
    /// 取得或设置字段组件绑定值改变的事件方法委托。
    /// </summary>
    Action<object> ValueChanged { get; set; }

    /// <summary>
    /// 取得或设置字段关联的栏位配置信息。
    /// </summary>
    ColumnInfo Column { get; set; }
}

/// <summary>
/// 自定义表单字段组件类。
/// </summary>
public abstract class CustomField : BaseComponent, ICustomField
{
    /// <summary>
    /// 取得或设置字段组件绑定的值。
    /// </summary>
    [Parameter] public object Value { get; set; }

    /// <summary>
    /// 取得或设置字段组件绑定值改变的事件方法委托。
    /// </summary>
    [Parameter] public Action<object> ValueChanged { get; set; }

    /// <summary>
    /// 取得或设置字段关联的栏位配置信息。
    /// </summary>
    [Parameter] public ColumnInfo Column { get; set; }
}