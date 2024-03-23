using System.Linq.Expressions;
using Known.Entities;
using Known.Helpers;

namespace Known;

public class ImportContext
{
    internal Context Context { get; set; }
    internal Database Database { get; set; }
    internal string BizId { get; set; }
    public string BizParam => GetBizIdValue(1);
    internal string TableName => GetBizIdValue(1);
    internal string ModuleId => GetBizIdValue(2);
    internal bool IsDictionary => !string.IsNullOrWhiteSpace(BizId) && BizId.StartsWith("Dictionary");

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

public abstract class ImportBase(ImportContext context)
{
    internal ImportContext ImportContext { get; } = context;
    public Context Context { get; } = context.Context;
    public Database Database { get; } = context.Database;
    public Language Language => Context?.Language;
    public List<ColumnInfo> Columns { get; } = [];

    public virtual void InitColumns() { }
    public virtual Task<Result> ExecuteAsync(SysFile file) => Result.SuccessAsync("");

    internal static ImportBase Create(ImportContext context)
    {
        if (context.IsDictionary)
            return new DictionaryImport(context);

        if (!Config.ImportTypes.TryGetValue(context.BizId, out Type type))
            return null;

        return Activator.CreateInstance(type, context) as ImportBase;
    }
}

public abstract class ImportBase<TItem>(ImportContext context) : ImportBase(context)
{
    protected void AddColumn<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        Columns.Add(column);
    }
}

public class ImportRow<TItem> : Dictionary<string, string>
{
    private readonly Context context;

    internal ImportRow(Context context)
    {
        this.context = context;
    }

    public string ErrorMessage { get; set; }

    public string GetValue<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var key = GetKey(selector);
        return GetValue(key);
    }

    public string GetValue<TValue>(Result vr, Expression<Func<TItem, TValue>> selector, bool required)
    {
        var key = GetKey(selector);
        return GetValue(vr, key, required);
    }

    public TValue GetValueT<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var key = GetKey(selector);
        return GetValue<TValue>(key);
    }

    public TValue GetValueT<TValue>(Result vr, Expression<Func<TItem, TValue>> selector, bool required)
    {
        var key = GetKey(selector);
        return GetValue<TValue>(vr, key, required);
    }

    public string GetValue(string key)
    {
        if (!ContainsKey(key))
            return string.Empty;

        return this[key];
    }

    public string GetValue(Result vr, string key, bool required)
    {
        var value = GetValue(key);
        if (required && string.IsNullOrWhiteSpace(value))
            vr.AddError(context.Language.Required(key));

        return value;
    }

    public T GetValue<T>(string key)
    {
        var value = GetValue(key);
        if (string.IsNullOrWhiteSpace(value))
            return default;

        return Utils.ConvertTo<T>(value);
    }

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
    public override async Task<Result> ExecuteAsync(SysFile file)
    {
        var tableName = ImportContext.TableName;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("TableName"));

        var module = await Database.QueryByIdAsync<SysModule>(ImportContext.ModuleId);
        if (module == null)
            return Result.Error(Language.Required("ModuleId"));

        if (module.Form == null || module.Form.Fields == null)
            return Result.Error(Language.Required("Form.Fields"));

        var models = new List<Dictionary<string, object>>();
        var result = ImportHelper.ReadFile<Dictionary<string, object>>(Context, file, item =>
        {
            var model = new Dictionary<string, object>();
            foreach (var field in module.Form.Fields)
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
                await db.SaveAsync(tableName, item);
            }
        });
    }
}