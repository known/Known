using System.Reflection;
using Known.Entities;
using Known.Services;

namespace Known;

public abstract class ImportBase
{
    protected ImportBase(Database database)
    {
        Database = database;
    }

    static ImportBase()
    {
        var type = typeof(SysDictionaryImport);
        importTypes[type.Name] = type;
    }

    public Database Database { get; }
    public string BizId { get; set; }
    public virtual List<ImportColumn> Columns { get; }

    public virtual Task<Result> ExecuteAsync(SysFile file) => Result.SuccessAsync("");

    private static readonly Dictionary<string, Type> importTypes = new();
    public static void Register(Assembly assembly)
    {
        var types = assembly.GetTypes();
        if (types == null || types.Length == 0)
            return;

        foreach (var item in types)
        {
            if (item.IsSubclassOf(typeof(ImportBase)))
                importTypes[item.Name] = item;
        }
    }

    internal static ImportBase Create(string bizId, Database database)
    {
        var name = GetImportName(bizId);
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (!importTypes.ContainsKey(name))
            return null;

        var type = importTypes[name];
        var import = Activator.CreateInstance(type, database) as ImportBase;
        import.BizId = bizId;
        return import;
    }

    internal static List<ImportColumn> GetImportColumns(string bizId)
    {
        var name = GetImportName(bizId);
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (!importTypes.ContainsKey(name))
            return null;

        var type = importTypes[name];
        var import = Activator.CreateInstance(type, new Database()) as ImportBase;
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

    public ImportColumn(string name, bool required, Type codeType)
    {
        Name = name;
        Required = required;
        var codes = Cache.GetCodes(codeType.Name);
        Note = $"填写：{string.Join(",", codes.Select(c => c.Code))}";
    }

    public string Name { get; }
    public bool Required { get; }
    public string Note { get; }
}

public class ImportRow : Dictionary<string, string>
{
    internal ImportRow() { }

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
            vr.AddError($"{key}不能为空！");

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
            vr.AddError($"{key}格式不正确！");

        return value;
    }
}