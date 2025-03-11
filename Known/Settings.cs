﻿namespace Known;

/// <summary>
/// 查询信息类。
/// </summary>
public class QueryInfo
{
    /// <summary>
    /// 构造函数，创建一个查询信息类的实例。
    /// </summary>
    public QueryInfo() { }

    /// <summary>
    /// 构造函数，创建一个查询信息类的实例，查询操作类型默认为包含于。
    /// </summary>
    /// <param name="id">查询字段ID。</param>
    /// <param name="value">查询字段值。</param>
    public QueryInfo(string id, string value) : this(id, QueryType.Contain, value) { }

    /// <summary>
    /// 构造函数，创建一个查询信息类的实例。
    /// </summary>
    /// <param name="id">查询字段ID。</param>
    /// <param name="type">查询操作类型。</param>
    /// <param name="value">查询字段值。</param>
    public QueryInfo(string id, QueryType type, string value)
    {
        Id = id;
        Type = type;
        Value = value;
    }

    internal QueryInfo(ColumnInfo column)
    {
        Id = column.Id;
        Type = QueryType.Contain;
        Value = "";
        if (!column.IsQueryAll)
        {
            var codes = Cache.GetCodes(column.Category);
            if (codes != null && codes.Count > 0)
                Value = codes[0].Code;
        }
    }

    /// <summary>
    /// 取得或设置查询字段ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置查询字段值。
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 取得或设置查询操作类型。
    /// </summary>
    public QueryType Type { get; set; }

    internal bool IsNew { get; set; }
    internal object ParamValue { get; set; }
}

/// <summary>
/// 系统布局模式枚举。
/// </summary>
public enum LayoutMode
{
    /// <summary>
    /// 纵向菜单布局。
    /// </summary>
    Vertical,
    /// <summary>
    /// 横向菜单布局。
    /// </summary>
    Horizontal,
    /// <summary>
    /// 浮动菜单布局。
    /// </summary>
    Float
}

/// <summary>
/// 用户系统设置信息类。
/// </summary>
public class UserSettingInfo
{
    /// <summary>
    /// 构造函数，创建一个系统设置信息类的实例。
    /// </summary>
    public UserSettingInfo()
    {
        LayoutMode = Known.LayoutMode.Vertical.ToString();
        Config.OnSetting?.Invoke(this);
    }

    /// <summary>
    /// 取得或设置系统当前字体大小。
    /// </summary>
    public string Size { get; set; }

    /// <summary>
    /// 取得或设置系统当前语言。
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// 取得或设置系统当前主题。
    /// </summary>
    public string Theme { get; set; }

    /// <summary>
    /// 取得或设置系统是否多标签页模式。
    /// </summary>
    public bool MultiTab { get; set; }

    /// <summary>
    /// 取得或设置系统多标签页是否顶行显示。
    /// </summary>
    public bool IsTopTab { get; set; }

    /// <summary>
    /// 取得或是设置最大标签数量。
    /// </summary>
    public int? MaxTabCount { get; set; }

    /// <summary>
    /// 取得或设置系统菜单是否是手风琴， 默认是。
    /// </summary>
    public bool Accordion { get; set; } = true;

    /// <summary>
    /// 取得或设置系统菜单是否收缩。
    /// </summary>
    public bool Collapsed { get; set; }

    /// <summary>
    /// 取得或设置系统菜单主题，默认亮色（Light）。
    /// </summary>
    public string MenuTheme { get; set; } = "Light";

    /// <summary>
    /// 取得或设置系统主题颜色。
    /// </summary>
    public string ThemeColor { get; set; } = "default";

    /// <summary>
    /// 取得或设置系统布局模式。
    /// </summary>
    public string LayoutMode { get; set; }

    /// <summary>
    /// 取得或设置表单打开方式，默认Modal-模态对话框。
    /// </summary>
    public string OpenType { get; set; } = nameof(FormOpenType.Modal);

    /// <summary>
    /// 取得或设置是否显示页面底部，默认否。
    /// </summary>
    public bool ShowFooter { get; set; }
}

/// <summary>
/// 表格列设置信息类。
/// </summary>
public class TableSettingInfo
{
    /// <summary>
    /// 取得或设置栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置栏位是否可见。
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// 取得或设置栏位宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置栏位显示顺序。
    /// </summary>
    public int Sort { get; set; }
}