﻿namespace Known.Components;

/// <summary>
/// 自定义Ant列组件类。
/// </summary>
public class AntCol : BaseComponent
{
    [CascadingParameter] private AntRow Row { get; set; }

    /// <summary>
    /// 取得或设置列的跨度。
    /// </summary>
    [Parameter] public int Span { get; set; }

    /// <summary>
    /// 取得或设置列的补偿宽度。
    /// </summary>
    [Parameter] public int Offset { get; set; }

    /// <summary>
    /// 取得或设置列的子内容。
    /// </summary>
    [Parameter] public RenderFragment ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var className = CssBuilder.Default("ant-col")
                          .AddClass($"ant-col-{Span}", Span > 0)
                          .AddClass($"ant-col-offset-{Offset}", Offset > 0)
                          .AddClass(Class)
                          .BuildClass();
        var style = CssBuilder.Default().AddStyle(Style);
        if (Row != null && Row.Gutter > 0)
        {
            style.Add("padding-left", $"{Row.Gutter / 2}px");
            style.Add("padding-right", $"{Row.Gutter / 2}px");
        }
        builder.Div().Class(className).Style(style.BuildStyle())
               .Child(() => builder.Fragment(ChildContent));
    }
}