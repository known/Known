﻿namespace Known.Extensions;

/// <summary>
/// 页面级组件扩展类。
/// </summary>
public static class PageExtension
{
    /// <summary>
    /// 呈现表格组件。
    /// </summary>
    /// <typeparam name="TItem">表格数据类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表格配置模型。</param>
    public static void Table<TItem>(this RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Component<TablePage<TItem>>().Set(c => c.Model, model).Build();
    }

    /// <summary>
    /// 构建树组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">树组件模型对象。</param>
    public static void Tree(this RenderTreeBuilder builder, TreeModel model)
    {
        builder.Component<AntTree>().Set(c => c.Model, model).Build();
    }

    /// <summary>
    /// 构建步骤组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">步骤组件模型对象。</param>
    public static void Steps(this RenderTreeBuilder builder, StepModel model)
    {
        builder.Component<DataSteps>().Set(c => c.Model, model).Build();
    }

    /// <summary>
    /// 构建标签页组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">标签页组件模型对象。</param>
    public static void Tabs(this RenderTreeBuilder builder, TabModel model)
    {
        builder.Component<DataTabs>().Set(c => c.Model, model).Build();
    }

    internal static void Toolbar(this RenderTreeBuilder builder, Action action)
    {
        builder.Div("kui-toolbar", action);
    }

    internal static void Toolbar(this RenderTreeBuilder builder, ToolbarModel model)
    {
        builder.Component<KToolbar>().Set(c => c.Model, model).Build();
    }

    internal static void Result(this RenderTreeBuilder builder, string status, string message)
    {
        builder.Component<AntDesign.Result>()
               .Set(c => c.Status, status)
               .Set(c => c.Title, status)
               .Set(c => c.SubTitle, message)
               .Build();
    }

    internal static void Page404(this UIService service, RenderTreeBuilder builder, string pageId)
    {
        builder.Result("404", $"{service.Language["Tip.Page404"]}PageId={pageId}");
    }
}