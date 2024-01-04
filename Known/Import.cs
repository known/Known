using Known.Entities;

namespace Known;

public abstract class ImportBase
{
    protected ImportBase(Context context, Database database)
    {
        Context = context;
        Database = database;
    }

    public Context Context { get; }
    public Database Database { get; }
    public Language Language => Context?.Language;
    public string BizId { get; set; }
    public virtual List<ImportColumn> Columns { get; }

    public virtual Task<Result> ExecuteAsync(SysFile file) => Result.SuccessAsync("");

    internal static ImportBase Create(string bizId, Context context, Database database)
    {
        var name = GetImportName(bizId);
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (!Config.ImportTypes.TryGetValue(name, out Type type))
            return null;

        var import = Activator.CreateInstance(type, context, database) as ImportBase;
        import.BizId = bizId;
        return import;
    }

    internal static List<ImportColumn> GetImportColumns(Context context, string bizId)
    {
        var name = GetImportName(bizId);
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (!Config.ImportTypes.TryGetValue(name, out Type type))
            return null;

        var import = Activator.CreateInstance(type, context, new Database()) as ImportBase;
        import.BizId = bizId;
        return import.Columns;
    }

    private static string GetImportName(string bizId) => bizId?.Split('_')[0];
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
            vr.AddError(context.Language.GetString("Valid.Required", key));

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