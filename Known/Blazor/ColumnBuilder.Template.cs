namespace Known.Blazor;

public partial class ColumnBuilder<TItem>
{
    /// <summary>
    /// 设置表格栏位呈现模板。
    /// </summary>
    /// <param name="template">呈现模板。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Template(RenderFragment template)
    {
        if (column != null)
            column.Template = template;
        return this;
    }

    /// <summary>
    /// 设置表格栏位呈现模板。
    /// </summary>
    /// <param name="template">呈现模板。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Template(Action<RenderTreeBuilder, TItem> template)
    {
        if (string.IsNullOrWhiteSpace(id))
            return this;

        if (Table != null)
            Table.Templates[id] = (row) => delegate (RenderTreeBuilder builder) { template(builder, row); };
        return this;
    }

    /// <summary>
    /// 设置表格栏位呈现为Tag组件。
    /// </summary>
    /// <param name="colorAction">颜色，默认自动。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Tag(Func<TItem, string> colorAction = null)
    {
        Template((b, r) =>
        {
            var text = r.Property(id)?.ToString();
            if (!string.IsNullOrWhiteSpace(column?.Category))
                text = Cache.GetCodeName(column.Category, text);
            var color = string.Empty;
            if (colorAction != null)
                color = colorAction.Invoke(r);
            b.Tag(text, color);
        });
        return this;
    }
}