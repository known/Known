namespace Known;

/// <summary>
/// 性别类型枚举。
/// </summary>
public enum GenderType
{
    /// <summary>
    /// 男。
    /// </summary>
    Male,
    /// <summary>
    /// 女。
    /// </summary>
    Female
}

/// <summary>
/// 菜单类型枚举。
/// </summary>
public enum MenuType
{
    /// <summary>
    /// 菜单。
    /// </summary>
    Menu,
    /// <summary>
    /// 页面。
    /// </summary>
    Page,
    /// <summary>
    /// 链接。
    /// </summary>
    Link,
    /// <summary>
    /// 原型。
    /// </summary>
    [CodeIgnore]
    Prototype
}

/// <summary>
/// 模块菜单类型枚举，适用于Admin插件。
/// </summary>
public enum ModuleType
{
    /// <summary>
    /// 菜单。
    /// </summary>
    Menu,
    /// <summary>
    /// 无代码表格页面。
    /// </summary>
    Page,
    /// <summary>
    /// 自定义页面。
    /// </summary>
    Custom,
    /// <summary>
    /// IFrame页面。
    /// </summary>
    IFrame
}

/// <summary>
/// 服务生命周期枚举。
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// 作用域。
    /// </summary>
    Scoped,
    /// <summary>
    /// 单例。
    /// </summary>
    Singleton,
    /// <summary>
    /// 瞬时。
    /// </summary>
    Transient
}