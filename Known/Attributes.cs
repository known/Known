namespace Known;

/// <summary>
/// WebApi特性类，用于标识服务类是否自动生成WebApi。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class WebApiAttribute : Attribute { }

/// <summary>
/// 服务特性类，用于标识服务类是否需要注入，生命周期默认Scoped。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped) : Attribute
{
    /// <summary>
    /// 取得服务生命周期类型。
    /// </summary>
    public ServiceLifetime Lifetime { get; } = lifetime;
}

/// <summary>
/// 客户端特性类，用于标识客户端类是否需要注入，生命周期默认Scoped。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ClientAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped) : Attribute
{
    /// <summary>
    /// 取得服务生命周期类型。
    /// </summary>
    public ServiceLifetime Lifetime { get; } = lifetime;
}

/// <summary>
/// 代码表特性类，用于标识常量类作为数据字典代码。
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CodeInfoAttribute : Attribute { }

/// <summary>
/// 菜单特性类，用于标识页面组件是否是模块菜单。
/// </summary>
/// <param name="parent">上级菜单。</param>
/// <param name="name">菜单名称。</param>
/// <param name="icon">菜单图标。</param>
/// <param name="sort">菜单排序。</param>
[AttributeUsage(AttributeTargets.Class)]
public class MenuAttribute(string parent, string name, string icon, int sort) : Attribute
{
    /// <summary>
    /// 取得或设置上级菜单。
    /// </summary>
    public string Parent { get; set; } = parent;

    /// <summary>
    /// 取得或设置菜单名称。
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// 取得或设置菜单图标。
    /// </summary>
    public string Icon { get; set; } = icon;

    /// <summary>
    /// 取得或设置菜单排序。
    /// </summary>
    public int Sort { get; set; } = sort;

    /// <summary>
    /// 取得或设置菜单URL。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的组件类型。
    /// </summary>
    public Type Page { get; set; }
}

/// <summary>
/// 动作特性类，用于标识方法是否需要角色权限控制。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute : Attribute { }

/// <summary>
/// 表格栏位特性类，用于编码方式设置实体类属性作为动态表格字段。
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    /// <summary>
    /// 构造函数，初始化一个栏位特性类的新实例。
    /// </summary>
    public ColumnAttribute() { }

    /// <summary>
    /// 构造函数，初始化一个栏位特性类类的新实例。
    /// </summary>
    /// <param name="field">数据库表字段名称。</param>
    public ColumnAttribute(string field)
    {
        Field = field;
    }

    /// <summary>
    /// 取得数据库表字段名称。
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是汇总字段。
    /// </summary>
    public bool IsSum { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是排序字段。
    /// </summary>
    public bool IsSort { get; set; }

    /// <summary>
    /// 取得或设置栏位默认排序方法（升序/降序）。
    /// </summary>
    public string DefaultSort { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查看连接（设为True，才可在线配置表单，为False，则默认为普通查询表格）。
    /// </summary>
    public bool IsViewLink { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查询条件。
    /// </summary>
    public bool IsQuery { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件下拉框是否显示【全部】。
    /// </summary>
    public bool IsQueryAll { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件组件类型。
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件数据字典类别。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置栏位固定列位置（left/right）。
    /// </summary>
    public string Fixed { get; set; }

    /// <summary>
    /// 取得或设置栏位宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置栏位显示顺序。
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置栏位对齐方式（left/center/right）。
    /// </summary>
    public string Align { get; set; }

    internal PropertyInfo Property { get; set; }
}

/// <summary>
/// 表单字段特性类，用于编码方式设置实体类属性作为动态表单字段。
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class FormAttribute() : Attribute
{
    /// <summary>
    /// 取得或设置字段所属行号，默认1。
    /// </summary>
    public int Row { get; set; } = 1;

    /// <summary>
    /// 取得或设置字段所属列号，默认1。
    /// </summary>
    public int Column { get; set; } = 1;

    /// <summary>
    /// 取得或设置字段组件类型，默认Text。
    /// </summary>
    public string Type { get; set; } = FieldType.Text.ToString();

    /// <summary>
    /// 取得或设置自定义字段组件类型名称。
    /// </summary>
    public string CustomField { get; set; }

    /// <summary>
    /// 取得或设置字段是否是只读。
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置字段控件占位符字符串。
    /// </summary>
    public string Placeholder { get; set; }
}