namespace Known.Core;

/// <summary>
/// 数据导入基类。
/// </summary>
/// <param name="context">导入上下文对象实例。</param>
public abstract class ImportBase(ImportContext context)
{
    internal ImportContext ImportContext { get; } = context;

    /// <summary>
    /// 取得系统上下文对象实例。
    /// </summary>
    public Context Context { get; } = context.Context;

    /// <summary>
    /// 取得上下文平台服务实例。
    /// </summary>
    public IPlatformService Platform => Context.Platform;

    /// <summary>
    /// 取得上下文数据库对象实例。
    /// </summary>
    public Database Database { get; } = context.Database;

    /// <summary>
    /// 取得上下文语言对象实例。
    /// </summary>
    public Language Language => Context?.Language;

    /// <summary>
    /// 取得导入栏位信息列表。
    /// </summary>
    public List<ColumnInfo> Columns { get; } = [];

    /// <summary>
    /// 初始化导入栏位虚方法。
    /// </summary>
    public virtual void InitColumns() { }

    /// <summary>
    /// 异步执行导入数据虚方法。
    /// </summary>
    /// <param name="file">导入文件对象。</param>
    /// <returns>导入结果。</returns>
    public virtual Task<Result> ExecuteAsync(AttachInfo file) => Result.SuccessAsync("");
}

/// <summary>
/// 数据导入上下文类。
/// </summary>
public class ImportContext
{
    internal Context Context { get; set; }
    internal Database Database { get; set; }
    internal string BizId { get; set; }
    internal bool IsDictionary => !string.IsNullOrWhiteSpace(BizId) && BizId.StartsWith("Dictionary");
    internal string BizParam => GetBizIdValue(1);

    private string GetBizIdValue(int index)
    {
        if (string.IsNullOrWhiteSpace(BizId))
            return string.Empty;

        var bizIds = BizId.Split('_');
        if (bizIds.Length > index)
            return bizIds[index];

        return string.Empty;
    }
}

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
            vr.AddError(context.Language.GetString("Valid.FormatInvalid", key));

        return value;
    }

    private string GetKey<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        return context.Language.GetString(column);
    }
}

class DictionaryImport(ImportContext context) : ImportBase(context)
{
    public override async Task<Result> ExecuteAsync(AttachInfo file)
    {
        var module = await Database.QueryByIdAsync<SysModule>(ImportContext.BizParam);
        if (module == null)
            return Result.Error(Language.Required("ModuleId"));

        var entity = DataHelper.ToEntity(module.EntityData);
        if (entity == null || string.IsNullOrWhiteSpace(entity.Id))
            return Result.Error(Language.Required("TableName"));

        var fields = module.GetFormFields();
        if (fields == null)
            return Result.Error(Language.Required("Form.Fields"));

        var models = new List<Dictionary<string, object>>();
        var result = ImportHelper.ReadFile<Dictionary<string, object>>(Context, file, item =>
        {
            var model = new Dictionary<string, object>();
            foreach (var field in fields)
            {
                if (field.Type == FieldType.Date || field.Type == FieldType.DateTime)
                    model[field.Id] = item.GetValue<DateTime?>(field.Name);
                else
                    model[field.Id] = item.GetValue(field.Name);
            }
            models.Add(model);
        });

        if (!result.IsValid)
            return result;

        return await Database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(entity.Id, item);
            }
        });
    }
}