namespace Known.Blazor;

/// <summary>
/// 表单行操作类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class FormRow<TItem> where TItem : class, new()
{
    internal FormRow(FormModel<TItem> form)
    {
        Form = form;
    }

    /// <summary>
    /// 取得表单行对应表单模型对象。
    /// </summary>
    public FormModel<TItem> Form { get; }

    /// <summary>
    /// 取得表单行关联的字段列表。
    /// </summary>
    public List<FieldModel<TItem>> Fields { get; } = [];

    /// <summary>
    /// 添加一列表单只读文本字段。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <param name="text">字段显示文本。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn(string id, string text) => AddColumn(id, b => b.Text(text));

    /// <summary>
    /// 添加一列表单呈现模板字段。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <param name="template">字段呈现模板。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn(string id, RenderFragment template) => AddColumn(new ColumnInfo(id, template));

    /// <summary>
    /// 添加一列表单字段。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="label">字段标题。</param>
    /// <param name="selector">字段属性选择表达式。</param>
    /// <param name="action">字段参数设置委托方法。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn<TValue>(string label, Expression<Func<TItem, TValue>> selector, Action<ColumnInfo> action = null)
    {
        return AddColumn(selector, c =>
        {
            c.Label = label;
            action?.Invoke(c);
        });
    }

    /// <summary>
    /// 添加一列表单字段。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="selector">字段属性选择表达式。</param>
    /// <param name="action">字段参数设置委托方法。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn<TValue>(Expression<Func<TItem, TValue>> selector, Action<ColumnInfo> action = null)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        action?.Invoke(column);
        return AddColumn(column);
    }

    /// <summary>
    /// 添加多列表单字段。
    /// </summary>
    /// <param name="columns">表单字段列对象列表。</param>
    /// <returns>表单行对象。</returns>
    public FormRow<TItem> AddColumn(params ColumnInfo[] columns)
    {
        foreach (var item in columns)
        {
            item.IsForm = true;
            Fields.Add(new FieldModel<TItem>(Form, item));
        }
        return this;
    }
}