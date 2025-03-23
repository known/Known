namespace Known.Data;

/// <summary>
/// 数据配置扩展类。
/// </summary>
public static class DbConfigExtension
{
    /// <summary>
    /// 添加数据实体模型配置。
    /// </summary>
    /// <param name="models">实体模型列表。</param>
    /// <param name="type">实体模型类型。</param>
    public static void Add(this List<DbModelInfo> models, Type type)
    {
        if (type.Name == nameof(EntityBase) || type.Name == nameof(FlowEntity))
            return;

        if (models.Exists(m => m.Type.FullName == type.FullName))
            return;

        models.Add(new DbModelInfo(type, [nameof(EntityBase.Id)]));
    }

    /// <summary>
    /// 添加数据实体模型配置。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="models">实体模型列表。</param>
    /// <param name="selector">主键选择表达式。</param>
    public static void Add<T>(this List<DbModelInfo> models, Expression<Func<T, object>> selector)
    {
        var type = typeof(T);
        if (models.Exists(m => m.Type.FullName == type.FullName))
            return;

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
        models.Add(new DbModelInfo(type, keys));
    }
}