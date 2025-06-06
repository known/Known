﻿using AntDesign;

namespace Known.Components;

/// <summary>
/// 表单数据字段项目组件类。
/// </summary>
public partial class DataItem
{
    private string ClassName => CssBuilder.Default()
                                          .AddClass("kui-block", Block)
                                          .AddClass("kui-inline", Inline)
                                          .BuildClass();
    private string LabelName => Language[Label];

    /// <summary>
    /// 取得或设置字段值类型，默认string。
    /// </summary>
    public Type Type { get; set; } = typeof(string);

    /// <summary>
    /// 取得或设置跨度。
    /// </summary>
    [Parameter] public int Span { get; set; }

    /// <summary>
    /// 取得或设置标题。
    /// </summary>
    [Parameter] public string Label { get; set; }

    /// <summary>
    /// 取得或设置标题模板。
    /// </summary>
    [Parameter] public RenderFragment LabelTemplate { get; set; }

    /// <summary>
    /// 取得或设置提示文字。
    /// </summary>
    [Parameter] public string Tooltip { get; set; }

    /// <summary>
    /// 取得或设置是否非Form字段。
    /// </summary>
    [Parameter] public bool NoForm { get; set; }

    /// <summary>
    /// 取得或设置是否必填。
    /// </summary>
    [Parameter] public bool Required { get; set; }

    /// <summary>
    /// 取得或设置多个元素是否每个元素单独一行显示。
    /// </summary>
    [Parameter] public bool Block { get; set; }

    /// <summary>
    /// 取得或设置多个元素是否一行内显示。
    /// </summary>
    [Parameter] public bool Inline { get; set; }

    /// <summary>
    /// 取得或设置验证规则集合。
    /// </summary>
    [Parameter] public FormValidationRule[] Rules { get; set; }

    /// <summary>
    /// 取得或设置子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }
}