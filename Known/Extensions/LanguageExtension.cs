namespace Known.Extensions;

/// <summary>
/// 语言服务扩展类。
/// </summary>
public static class LanguageExtension
{
    /// <summary>
    /// 获取查询结果。
    /// </summary>
    /// <param name="infos">语言信息列表。</param>
    /// <param name="criteria">查询条件。</param>
    /// <returns>查询结果。</returns>
    public static PagingResult<LanguageInfo> ToQueryResult(this List<LanguageInfo> infos, PagingCriteria criteria)
    {
        infos = [.. infos.Contains(m => m.Name, criteria)];
        return infos.ToPagingResult(criteria);
    }

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