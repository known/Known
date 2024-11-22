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
    public static void FormAction(this RenderTreeBuilder builder, Action child)
    {
        builder.Div("kui-form-action", child);
    }

    /// <summary>
    /// 呈现表单标题。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">表单标题。</param>
    public static void FormTitle(this RenderTreeBuilder builder, string text)
    {
        builder.Component<KTitle>().Set(c => c.Text, text).Build();
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
            builder.Label().Class("legend").Child(title).Close();
            builder.Div("body", child);
        });
    }

    /// <summary>
    /// 构建表格查询表单组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表格组件模型对象。</param>
    internal static void Query(this RenderTreeBuilder builder, TableModel model)
    {
        builder.Component<QueryForm>().Set(c => c.Model, model).Build();
    }
}