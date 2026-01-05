namespace Known;

/// <summary>
/// 数据实体基类。
/// </summary>
public class BaseEntity
{
    internal Dictionary<string, object> Original { get; set; }

    /// <summary>
    /// 取得或设置是否是新增实体。
    /// </summary>
    public virtual bool IsNew { get; set; } = true;

    internal void SetOriginal(Dictionary<string, object> original)
    {
        IsNew = false;
        Original = original;
    }

    internal bool IsChanged(string propertyName, object value)
    {
        if (Original == null || !Original.TryGetValue(propertyName, out object orgValue))
            return true;
        if (orgValue == null)
            return true;

        return !orgValue.Equals(value);
    }
}

/// <summary>
/// 数据实体基类，主键ID为泛型。
/// </summary>
/// <typeparam name="TKey">主键ID类型。</typeparam>
public class EntityBase<TKey> : BaseEntity
{
    /// <summary>
    /// 构造函数，创建一个数据实体类的实例。
    /// </summary>
    public EntityBase()
    {
        IsNew = true;
        Id = Utils.GetNextId<TKey>();
    }

    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    [Key] public TKey Id { get; set; }

    /// <summary>
    /// 填充实体对应的数据传输对象属性。
    /// </summary>
    /// <param name="model">数据传输对象。</param>
    public virtual void FillModel(object model)
    {
        var type = GetType();
        var baseFields = TypeHelper.GetBaseFields();
        var properties = TypeHelper.Properties(type);
        var modelProperties = TypeHelper.Properties(model.GetType()).ToList();
        foreach (var item in properties)
        {
            if (baseFields.Exists(d => d.Id == item.Name && d.Id != nameof(EntityBase.Extension)) || item.Name == nameof(IsNew))
                continue;

            var modelProperty = modelProperties.FirstOrDefault(p => p.Name == item.Name);
            if (modelProperty == null)
                continue;

            var value = item.GetValue(this, null);
            var data = modelProperty.GetValue(model, null);
            if ((value != null && !value.Equals(data)) ||
                (value != null && data == null) ||
                (value == null && data != null))
                item.SetValue(this, data);
        }
    }

    /// <summary>
    /// 实体类对象的数据合法性校验。
    /// </summary>
    /// <param name="context">系统上下文对象。</param>
    /// <returns>校验结果。</returns>
    public virtual Result Validate(Context context)
    {
        var type = GetType();
        var properties = TypeHelper.Properties(type);
        var dicError = new Dictionary<string, List<string>>();

        foreach (var pi in properties)
        {
            var value = pi.GetValue(this, null);
            var errors = new List<string>();
            pi.Validate(context.Language, value, errors);
            if (errors.Count > 0)
                dicError.Add(pi.Name, errors);
        }

        if (dicError.Count > 0)
        {
            var result = Result.Error("", dicError);
            foreach (var item in dicError.Values)
            {
                item.ForEach(result.AddError);
            }
            return result;
        }

        return Result.Success("");
    }
}

/// <summary>
/// 数据实体基类，主键ID为字符串类型。
/// </summary>
public class EntityBase : EntityBase<string>
{
    private Dictionary<string, object> extension;

    /// <summary>
    /// 构造函数，创建一个数据实体类的实例。
    /// </summary>
    public EntityBase() : base()
    {
        CreateBy = "temp";
        CreateTime = DateTime.Now;
        Version = 1;
        AppId = "temp";
        CompNo = "temp";
    }

    /// <summary>
    /// 取得或设置实体创建人。
    /// </summary>
    [DisplayName("创建人")]
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置实体创建时间。
    /// </summary>
    [DisplayName("创建时间")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 取得或设置实体最近一次修改人。
    /// </summary>
    [DisplayName("修改人")]
    public string ModifyBy { get; set; }

    /// <summary>
    /// 取得或设置实体最近一次修改时间。
    /// </summary>
    [DisplayName("修改时间")]
    public DateTime? ModifyTime { get; set; }

    /// <summary>
    /// 取得或设置实体版本号。
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 取得或设置实体扩展信息的JSON字符串。
    /// </summary>
    public string Extension { get; set; }

    /// <summary>
    /// 取得或设置实体关联的系统ID。
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置实体关联的企业租户编码。
    /// </summary>
    public string CompNo { get; set; }

    /// <summary>
    /// 获取实体的扩展属性对象。
    /// </summary>
    /// <typeparam name="T">扩展属性类型。</typeparam>
    /// <param name="key">扩展属性键。</param>
    /// <returns>扩展属性对象。</returns>
    internal T GetExtension<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(Extension))
            return default;

        extension = Utils.FromJson<Dictionary<string, object>>(Extension);
        if (!extension.TryGetValue(key, out object value))
            return default;

        return Utils.ConvertTo<T>(value);
    }

    /// <summary>
    /// 设置实体扩展属性对象。
    /// </summary>
    /// <param name="key">扩展属性键。</param>
    /// <param name="value">扩展属性对象。</param>
    internal void SetExtension(string key, object value)
    {
        extension ??= [];
        extension[key] = value;
        Extension = Utils.ToJson(extension);
    }
}