namespace Known;

[AttributeUsage(AttributeTargets.Property)]
class LanguageAttribute(string code, string icon, bool isDefault = false, bool enabled = false) : Attribute
{
    public string Code { get; } = code;
    public string Icon { get; } = icon;
    public bool Default { get; } = isDefault;
    public bool Enabled { get; } = enabled;
}

/// <summary>
/// 匿名访问特性类，用于标识控制器或方法允许匿名访问。
/// </summary>
public class AnonymousAttribute : Attribute { }

/// <summary>
/// 任务特性类，用于标识任务类。
/// </summary>
/// <param name="bizType">任务关联的业务类型。</param>
[AttributeUsage(AttributeTargets.Class)]
public class TaskAttribute(string bizType) : Attribute
{
    /// <summary>
    /// 取得任务类关联的Task表的业务类型(BizType)。
    /// </summary>
    public string BizType { get; } = bizType;
}

/// <summary>
/// 导入特性类，用于标识导入类。
/// </summary>
/// <param name="type">导入关联的数据类型。</param>
[AttributeUsage(AttributeTargets.Class)]
public class ImportAttribute(Type type) : Attribute
{
    /// <summary>
    /// 取得导入类关联的数据类型，通常是列表和表单页面的数据类型。
    /// </summary>
    public Type Type { get; } = type;
}

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
/// 代码表项目忽略特性类，用于标识常量类或枚举项目忽略作为数据字典代码。
/// </summary>
public class CodeIgnoreAttribute : Attribute { }

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
    /// 取得或设置菜单角色。
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置菜单关联的组件类型。
    /// </summary>
    public Type Page { get; set; }
}

/// <summary>
/// 移动端菜单特性类，用于标识页面组件是否是移动端菜单。
/// </summary>
/// <param name="name">菜单名称。</param>
/// <param name="icon">菜单图标。</param>
/// <param name="sort">菜单排序。</param>
/// <param name="target">菜单目标位置，Tab/Menu，默认Menu。</param>
[AttributeUsage(AttributeTargets.Class)]
public class AppMenuAttribute(string name, string icon, int sort, string target = "Menu") : MenuAttribute("", name, icon, sort)
{
    /// <summary>
    /// 取得或设置菜单目标位置，Tab/Menu，默认Menu。
    /// </summary>
    public string Target { get; set; } = target;

    /// <summary>
    /// 取得或设置首页菜单图标背景颜色。
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// 取得或设置返回页面URL。
    /// </summary>
    public string BackUrl { get; set; }
}

/// <summary>
/// 角色特性类，用于标识组件类需要在角色管理中配置按钮权限。
/// </summary>
/// <param name="name">组件名称。</param>
[AttributeUsage(AttributeTargets.Class)]
public class RoleAttribute(string name) : Attribute
{
    /// <summary>
    /// 取得角色组件名称。
    /// </summary>
    public string Name { get; } = name;
}

/// <summary>
/// 标签组件角色特性类，用于标识页面标签组件类需要在角色管理中配置权限。
/// </summary>
/// <param name="parent">页面组件类型。</param>
/// <param name="name">组件名称。</param>
[AttributeUsage(AttributeTargets.Class)]
public class TabRoleAttribute(Type parent, string name) : Attribute
{
    /// <summary>
    /// 取得上级页面组件类型。
    /// </summary>
    public Type Parent { get; } = parent;

    /// <summary>
    /// 取得角色组件名称。
    /// </summary>
    public string Name { get; } = name;
}

/// <summary>
/// 动作特性类，用于标识方法是否需要角色权限控制。
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute : Attribute
{
    /// <summary>
    /// 取得或设置操作按钮图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置操作按钮名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置操作按钮提示信息。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置操作样式，如：primary，danger，default，dashed，link，text等。
    /// </summary>
    public string Style { get; set; }

    /// <summary>
    /// 取得或设置操作按钮是否可见，默认可见。
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 取得或设置操作按钮分组。
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// 取得或设置操作按钮所属选项卡集合。
    /// </summary>
    public string[] Tabs { get; set; }
}

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
    /// 取得或设置栏位文本超出宽度是否显示省略号，显示则文本不换行。
    /// </summary>
    public bool Ellipsis { get; set; }

    /// <summary>
    /// 取得或设置栏位是否可见，默认True。
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// 取得或设置栏位是否是汇总字段。
    /// </summary>
    public bool IsSum { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是排序字段，默认排序。
    /// </summary>
    public bool IsSort { get; set; } = true;

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
    /// 取得或设置栏位查询条件默认值。
    /// </summary>
    public string QueryValue { get; set; }

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
    public int Width { get; set; }

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
    /// 取得或设置字段组件类型。
    /// </summary>
    public string Type { get; set; }

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

    /// <summary>
    /// 取得或设置字段默认值。
    /// </summary>
    public string FieldValue { get; set; }

    /// <summary>
    /// 取得或设置文本域组件行数，默认3。
    /// </summary>
    public uint Rows { get; set; } = 3;

    /// <summary>
    /// 取得或设置计量单位。
    /// </summary>
    public string Unit { get; set; }
}