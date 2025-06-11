namespace Known.Blazor;

public partial class ColumnBuilder<TItem>
{
    /// <summary>
    /// 设置表格栏位是否只读。
    /// </summary>
    /// <param name="readOnly">是否只读。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> ReadOnly(bool readOnly)
    {
        if (column != null)
            column.ReadOnly = readOnly;
        return this;
    }

    /// <summary>
    /// 设置表格栏位字段类。
    /// </summary>
    /// <param name="type">字段类型。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Type(FieldType type)
    {
        if (column != null)
            column.Type = type;
        if (allColumn != null)
            allColumn.Type = type;
        return this;
    }

    /// <summary>
    /// 设置表单组件占位符。
    /// </summary>
    /// <param name="placeholder">占位符。</param>
    /// <returns></returns>
    public ColumnBuilder<TItem> Placeholder(string placeholder)
    {
        if (column != null)
            column.Placeholder = placeholder;
        return this;
    }

    /// <summary>
    /// 设置表单附件字段的导入模板URL。
    /// </summary>
    /// <param name="templateUrl">导入模板URL。</param>
    /// <returns></returns>
    public ColumnBuilder<TItem> TemplateUrl(string templateUrl)
    {
        if (column != null)
            column.TemplateUrl = templateUrl;
        return this;
    }
}