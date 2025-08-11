namespace Known.Extensions;

/// <summary>
/// 表单组件扩展类。
/// </summary>
public static class FormExtension
{
    /// <summary>
    /// 构建表单组件。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表单组件模型对象。</param>
    public static void Form<TItem>(this RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new()
    {
        builder.Component<DataForm<TItem>>().Set(c => c.Model, model).Build();
    }

    /// <summary>
    /// 呈现表单页面内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    public static void FormPage(this RenderTreeBuilder builder, Action child)
    {
        builder.Div("kui-form-page", child);
    }

    /// <summary>
    /// 呈现表单页面操作按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    public static void FormPageButton(this RenderTreeBuilder builder, Action child)
    {
        builder.Div("kui-form-page-button", child);
    }

    /// <summary>
    /// 呈现表单操作按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    public static void FormButton(this RenderTreeBuilder builder, Action child)
    {
        builder.Div("kui-form-button", child);
    }

    /// <summary>
    /// 呈现表单操作按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="child">子内容委托。</param>
    /// <param name="left">左侧内容委托。</param>
    public static void FormAction(this RenderTreeBuilder builder, Action child, RenderFragment left = null)
    {
        builder.Div("kui-form-action", () =>
        {
            builder.Div("left", () => builder.Fragment(left));
            builder.Div("right", child);
        });
    }

    /// <summary>
    /// 呈现表单标题。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">表单标题。</param>
    /// <param name="subText">表单子标题。</param>
    public static void FormTitle(this RenderTreeBuilder builder, string text, string subText = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        builder.Component<KTitle>().Set(c => c.Text, text).Set(c => c.SubText, subText).Build();
    }

    /// <summary>
    /// 呈现一个分组框。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="title">分组框标题。</param>
    /// <param name="child">子内容委托。</param>
    public static void GroupBox(this RenderTreeBuilder builder, string title, Action child)
    {
        builder.Div("kui-group-box", () =>
        {
            builder.Label().Class("legend").Child(title);
            builder.Div("body", child);
        });
    }

    /// <summary>
    /// 构建表格查询表单组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表格组件模型对象。</param>
    public static void Query(this RenderTreeBuilder builder, TableModel model)
    {
        builder.Component<QueryForm>().Set(c => c.Model, model).Build();
    }
}