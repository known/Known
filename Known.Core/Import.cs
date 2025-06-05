using System.Linq.Expressions;

namespace Known;

/// <summary>
/// 数据导入泛型基类。
/// </summary>
/// <typeparam name="TItem">导入数据类型。</typeparam>
/// <param name="context">导入上下文对象实例。</param>
public abstract class ImportBase<TItem>(ImportContext context) : ImportBase(context)
{
    /// <summary>
    /// 添加导入栏位信息。
    /// </summary>
    /// <typeparam name="TValue">栏位属性类型。</typeparam>
    /// <param name="selector">数据栏位属性选择表达式。</param>
    protected void AddColumn<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        Columns.Add(column);
    }
}

/// <summary>
/// 导入数据行字典扩展类。
/// </summary>
/// <typeparam name="TItem">导入数据类型。</typeparam>
public class ImportRow<TItem> : Dictionary<string, string>
{
    private readonly Context context;

    internal ImportRow(Context context)
    {
        this.context = context;
    }

    /// <summary>
    /// 取得或设置数据校验错误信息。
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 获取泛型类型字段值。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="selector">数据栏位属性选择表达式。</param>
    /// <returns>字段值。</returns>
    public string GetValue<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var key = GetKey(selector);
        return GetValue(key);
    }

    /// <summary>
    /// 获取泛型类型字段值。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="vr">验证结果对象。</param>
    /// <param name="selector">数据栏位属性选择表达式。</param>
    /// <param name="required">是否必填。</param>
    /// <returns>字段值。</returns>
    public string GetValue<TValue>(Result vr, Expression<Func<TItem, TValue>> selector, bool required)
    {
        var key = GetKey(selector);
        return GetValue(vr, key, required);
    }

    /// <summary>
    /// 获取泛型类型字段值。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="selector">数据栏位属性选择表达式。</param>
    /// <returns>字段值。</returns>
    public TValue GetValueT<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var key = GetKey(selector);
        return GetValue<TValue>(key);
    }

    /// <summary>
    /// 获取泛型类型字段值。
    /// </summary>
    /// <typeparam name="TValue">字段属性类型。</typeparam>
    /// <param name="vr">验证结果对象。</param>
    /// <param name="selector">数据栏位属性选择表达式。</param>
    /// <param name="required">是否必填。</param>
    /// <returns>字段值。</returns>
    public TValue GetValueT<TValue>(Result vr, Expression<Func<TItem, TValue>> selector, bool required)
    {
        var key = GetKey(selector);
        return GetValue<TValue>(vr, key, required);
    }

    /// <summary>
    /// 获取字符串类型字段值。
    /// </summary>
    /// <param name="key">字段键。</param>
    /// <returns>字段值。</returns>
    public string GetValue(string key)
    {
        if (!ContainsKey(key))
            return string.Empty;

        return this[key];
    }

    /// <summary>
    /// 获取字符串类型字段值。
    /// </summary>
    /// <param name="vr">验证结果对象。</param>
    /// <param name="key">字段键。</param>
    /// <param name="required">是否必填。</param>
    /// <returns>字段值。</returns>
    public string GetValue(Result vr, string key, bool required)
    {
        var value = GetValue(key);
        if (required && string.IsNullOrWhiteSpace(value))
            vr.AddError(context.Language.Required(key));

        return value;
    }

    /// <summary>
    /// 获取泛型类型字段值。
    /// </summary>
    /// <typeparam name="T">字段属性类型。</typeparam>
    /// <param name="key">字段键。</param>
    /// <returns>字段值。</returns>
    public T GetValue<T>(string key)
    {
        var value = GetValue(key);
        if (string.IsNullOrWhiteSpace(value))
            return default;

        return Utils.ConvertTo<T>(value);
    }

    /// <summary>
    /// 获取泛型类型字段值。
    /// </summary>
    /// <typeparam name="T">字段属性类型。</typeparam>
    /// <param name="vr">验证结果对象。</param>
    /// <param name="key">字段键。</param>
    /// <param name="required">是否必填。</param>
    /// <returns>字段值。</returns>
    public T GetValue<T>(Result vr, string key, bool required)
    {
        var value = GetValue<T>(key);
        if (required && value == null)
            vr.AddError(context.Language[CoreLanguage.TipFormatInvalid].Replace("{label}", key));

        return value;
    }

    private string GetKey<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        return context.Language.GetFieldName<TItem>(column);
    }
}