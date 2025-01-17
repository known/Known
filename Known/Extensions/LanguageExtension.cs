namespace Known.Extensions;

/// <summary>
/// 语言服务扩展类。
/// </summary>
public static class LanguageExtension
{
    /// <summary>
    /// 获取字段语言名称。
    /// </summary>
    /// <typeparam name="TItem">实体类型。</typeparam>
    /// <param name="language">多语言对象。</param>
    /// <param name="column">栏位信息。</param>
    /// <returns></returns>
    public static string GetFieldName<TItem>(this Language language, ColumnInfo column)
    {
        return language?.GetFieldName(column, typeof(TItem));
    }

    /// <summary>
    /// 获取字段语言名称。
    /// </summary>
    /// <param name="language">多语言对象。</param>
    /// <param name="column">栏位信息。</param>
    /// <param name="type">实体类型。</param>
    /// <returns></returns>
    public static string GetFieldName(this Language language, ColumnInfo column, Type type = null)
    {
        if (!string.IsNullOrEmpty(column.Label))
            return column.Label;

        if (!string.IsNullOrEmpty(column.DisplayName))
            return column.DisplayName;

        return language?.GetString(column, type);
    }
}