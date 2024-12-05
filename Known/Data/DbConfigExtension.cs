namespace Known.Data;

/// <summary>
/// 数据配置扩展类。
/// </summary>
public static class DbConfigExtension
{
    /// <summary>
    /// 添加数据模型配置。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="models"></param>
    /// <param name="selector"></param>
    public static void Add<T>(this List<DbModelInfo> models, Expression<Func<T, object>> selector)
    {
        var keys = new List<string>();
        if (selector.Body is MemberExpression)
        {
            var member = (MemberExpression)selector.Body;
            keys.Add(member.Member.Name);
        }
        else if (selector.Body is NewExpression)
        {
            var member = (NewExpression)selector.Body;
            var arguments = member.Arguments.Select(a => ((MemberExpression)a).Member.Name);
            keys.AddRange(arguments);
        }
        models.Add(new DbModelInfo { Type = typeof(T), Keys = keys });
    }
}