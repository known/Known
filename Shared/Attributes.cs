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