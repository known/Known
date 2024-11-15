namespace Known.Components;

/// <summary>
/// 自定义数据表单组件类。
/// </summary>
/// <typeparam name="TItem">表单数据对象类型。</typeparam>
public class DataForm<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得或设置表单组件模型对象实例。
    /// </summary>
    [Parameter] public FormModel<TItem> Model { get; set; }

    /// <summary>
    /// 呈现表单组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (Model.Header != null)
            builder.Fragment(Model.Header);
        builder.Component<AntForm<TItem>>()
               .Set(c => c.Form, Model)
               .Set(c => c.ChildContent, this.BuildTree<TItem>(BuildContent))
               .Build();
    }

    private void BuildContent(RenderTreeBuilder builder, TItem item)
    {
        if (Model.Rows == null || Model.Rows.Count == 0)
            return;

        foreach (var row in Model.Rows)
        {
            builder.Component<AntFormRow<TItem>>().Set(c => c.Row, row).Build();
        }
    }
}