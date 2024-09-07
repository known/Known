namespace Known.Data;

/// <summary>
/// 查询扩展类。
/// </summary>
public static class QueryExtension
{
    /// <summary>
    /// 创建一个查询建造者实例。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="db">数据库访问对象。</param>
    /// <returns>查询建造者实例。</returns>
    public static QueryBuilder<T> Query<T>(this Database db) where T : class, new() => new(db);
}