namespace Known.Blazor;

partial class FormModel<TItem>
{
    /// <summary>
    /// 取得表单字段代码表字典。
    /// </summary>
    public Dictionary<string, List<CodeInfo>> Codes { get; } = [];

    /// <summary>
    /// 取得表单字段模型信息字典。
    /// </summary>
    public Dictionary<string, FieldModel<TItem>> Fields { get; } = [];

    /// <summary>
    /// 取得或设置表单字段值改变时调用的委托。
    /// </summary>
    public Action<string> OnFieldChanged { get; set; }

    /// <summary>
    /// 以编码方式添加表单行布局。
    /// </summary>
    /// <param name="action">行内子组件委托。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddRow(Action<FormRow<TItem>> action = null)
    {
        var row = new FormRow<TItem>(this);
        action?.Invoke(row);
        Rows.Add(row);
        return row;
    }

    /// <summary>
    /// 根据属性选择表达式返回表单字段栏位建造者。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="selector">属性选择表达式。</param>
    /// <returns>字段栏位建造者。</returns>
    public ColumnBuilder<TItem> Field<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = columns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column);
    }

    internal List<CodeInfo> GetCodes(ColumnInfo column)
    {
        if (column == null || string.IsNullOrWhiteSpace(column.Category))
            return null;

        if (Codes == null || Codes.Count == 0)
            return null;

        if (Codes.TryGetValue(column.Category, out List<CodeInfo> value))
            return value;

        return null;
    }
}