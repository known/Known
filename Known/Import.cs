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

public abstract class ImportBase
{
    protected ImportBase(ImportContext context)
    {
        ImportContext = context;
        Context = context.Context;
        Database = context.Database;
    }

    internal ImportContext ImportContext { get; }
    public Context Context { get; }
    public Database Database { get; }
    public Language Language => Context?.Language;
    public virtual List<ImportColumn> Columns { get; }

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

public class ImportColumn
{
    public ImportColumn(string name, bool required = false, string note = null)
    {
        Name = name;
        Required = required;
        Note = note;
    }

    public ImportColumn(Context context, string name, bool required, Type codeType)
    {
        Name = name;
        Required = required;
        var codes = Cache.GetCodes(codeType.Name);
        Note = context.Language["Import.TemplateFill"].Replace("{text}", $"{string.Join(",", codes.Select(c => c.Code))}");
    }

    public string Name { get; }
    public bool Required { get; }
    public string Note { get; }
}

public class ImportRow : Dictionary<string, string>
{
    private readonly Context context;

    internal ImportRow(Context context)
    {
        this.context = context;
    }

    public string ErrorMessage { get; set; }

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
        var result = ImportHelper.ReadFile(Context, file, item =>
        {
            var model = new Dictionary<string, object>();
            foreach (var field in module.Form.Fields)
            {
                if (field.Type == FieldType.Date)
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