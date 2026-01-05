namespace Known.Data;

/// <summary>
/// 数据模型配置信息类，适用于EFCore配置模型。
/// </summary>
public class DbModelInfo
{
    /// <summary>
    /// 构造函数，初始化数据模型配置信息类。
    /// </summary>
    /// <param name="type">实体类型。</param>
    /// <param name="keys">主键列表。</param>
    public DbModelInfo(Type type, List<string> keys = null)
    {
        Type = type;
        Keys = keys;
        Fields = GetFields(true);
        if (keys == null)
            Keys = [.. Fields.Where(f => f.IsKey).Select(f => f.Id)];
    }

    /// <summary>
    /// 取得实体类型。
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// 取得主键字段列表。
    /// </summary>
    public List<string> Keys { get; }

    /// <summary>
    /// 取得字段列表。
    /// </summary>
    public List<FieldInfo> Fields { get; }

    /// <summary>
    /// 获取字段列表。
    /// </summary>
    /// <param name="includeBase">是否包含基类字段。</param>
    /// <returns></returns>
    public List<FieldInfo> GetFields(bool includeBase = false)
    {
        var isEntity = Type.IsSubclassOf(typeof(EntityBase));
        var infos = isEntity && includeBase ? TypeHelper.GetBaseFields() : [];
        var fields = TypeCache.Fields(Type);
        if (fields != null && fields.Count > 0)
        {
            foreach (var item in fields)
            {
                var property = item.Property;
                if (infos.Exists(f => f.Id == item.Name) || property == null)
                    continue;

                if (property.CanRead && property.CanWrite && !property.GetMethod.IsVirtual)
                {
                    var field = item.GetField();
                    infos.Add(field);
                }
            }
        }
        return infos;
    }
}