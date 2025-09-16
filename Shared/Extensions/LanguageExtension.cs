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
        infos = [.. infos.Contains(m => m.Chinese, criteria)];
        return infos.ToPagingResult(criteria);
    }

    internal static void Add(this List<LanguageInfo> infos, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (infos.Exists(l => l.Chinese == name))
            return;

        infos.Add(new LanguageInfo { Chinese = name });
    }

    internal static void AddEnum(this List<LanguageInfo> infos, Type type)
    {
        var values = Enum.GetValues(type);
        foreach (Enum item in values)
        {
            var code = Enum.GetName(type, item);
            var name = item.GetDescription();
            if (code == name)
                continue;

            infos.Add(name);
        }
    }

    internal static void AddAttribute(this List<LanguageInfo> infos, Type type)
    {
        var properties = TypeHelper.Properties(type);
        foreach (var property in properties)
        {
            var name = property.DisplayName();
            infos.Add(name);
        }
    }

    internal static void AddConstant(this List<LanguageInfo> infos, Type type)
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        foreach (var field in fields)
        {
            if (field.IsLiteral && !field.IsInitOnly)
            {
                var name = field.GetValue(null)?.ToString();
                infos.Add(name);
            }
        }
    }
}