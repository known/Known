namespace Known.Extensions;

/// <summary>
/// 列表数据查询扩展类。
/// </summary>
public static class QueryExtension
{
    /// <summary>
    /// 获取日期查询Where条件语句。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="criteria">分页查询条件。</param>
    /// <param name="date">日期查询条件值。</param>
    /// <param name="id">字段ID。</param>
    /// <param name="field">数据库字段。</param>
    /// <returns></returns>
    public static string GetDateWhere(this Database db, PagingCriteria criteria, string date, string id, string field = null)
    {
        if (string.IsNullOrWhiteSpace(date))
            return string.Empty;

        var where = string.Empty;
        var dates = date.Split('~');
        if (string.IsNullOrWhiteSpace(field))
            field = id;

        if (dates.Length > 0 && !string.IsNullOrWhiteSpace(dates[0]))
        {
            where += $" and {field}>=@L{id}";
            var query = criteria.SetQuery($"L{id}", QueryType.Between, dates[0]);
            query.ParamValue = QueryHelper.GetStartDateValue(db, dates[0]);
        }

        if (dates.Length > 1 && !string.IsNullOrWhiteSpace(dates[1]))
        {
            where += $" and {field}<=@G{id}";
            var query = criteria.SetQuery($"G{id}", QueryType.Between, dates[1]);
            query.ParamValue = QueryHelper.GetEndDateValue(db, dates[1]);
        }

        return where;
    }

    /// <summary>
    /// 列表数据包含查询扩展方法。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <typeparam name="TProperty">属性类型。</typeparam>
    /// <param name="source">数据源。</param>
    /// <param name="selector">属性选择器。</param>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    public static IEnumerable<T> Contains<T, TProperty>(this IEnumerable<T> source, Expression<Func<T, TProperty>> selector, PagingCriteria criteria)
    {
        var propertyName = GetPropertyName(selector);
        var queryValue = criteria.GetQueryValue(propertyName);
        if (!string.IsNullOrWhiteSpace(queryValue))
        {
            var compiledSelector = selector.Compile();
            source = source.Where(item =>
            {
                var value = compiledSelector(item)?.ToString();
                return value?.Contains(queryValue, StringComparison.OrdinalIgnoreCase) == true;
            });
        }
        return source;
    }

    /// <summary>
    /// 列表数据等于查询扩展方法
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <typeparam name="TProperty">属性类型。</typeparam>
    /// <param name="source">数据源。</param>
    /// <param name="selector">属性选择器。</param>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    public static IEnumerable<T> Equals<T, TProperty>(this IEnumerable<T> source, Expression<Func<T, TProperty>> selector, PagingCriteria criteria)
    {
        var propertyName = GetPropertyName(selector);
        var queryValue = criteria.GetQueryValue(propertyName);
        if (!string.IsNullOrWhiteSpace(queryValue))
        {
            var compiledSelector = selector.Compile();
            source = source.Where(item =>
            {
                var value = compiledSelector(item)?.ToString();
                return value?.Equals(queryValue, StringComparison.OrdinalIgnoreCase) == true;
            });
        }
        return source;
    }

    internal static string GetFieldName(this PagingCriteria criteria, string key)
    {
        var field = criteria.EntityType?.GetFieldName(key);
        if (string.IsNullOrWhiteSpace(field))
            field = key;
        if (criteria.Fields.TryGetValue(key, out string value))
            field = value;
        return field;
    }

    private static string GetFieldName(this Type entityType, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return string.Empty;

        if (entityType.IsDictionary())
            return propertyName;

        var property = TypeCache.Property(entityType, propertyName);
        return property?.GetFieldName();
    }

    private static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> selector)
    {
        if (selector.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }
        throw new ArgumentException("Selector must be a member expression.");
    }
}