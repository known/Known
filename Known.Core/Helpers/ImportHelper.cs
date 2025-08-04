using Known.Cells;
using Known.Imports;

namespace Known.Helpers;

/// <summary>
/// 数据导入帮助者类。
/// </summary>
public sealed class ImportHelper
{
    /// <summary>
    /// 导入业务类型。
    /// </summary>
    public const string BizType = "ImportFiles";

    private ImportHelper() { }

    internal static Task ExecuteAsync(TaskInfo task) => TaskHelper.RunAsync(task, ExecuteAsync);

    /// <summary>
    /// 读取导入文件并处理导入逻辑。
    /// </summary>
    /// <typeparam name="TItem">导入数据类型。</typeparam>
    /// <param name="context">上下文对象。</param>
    /// <param name="file">导入文件对象。</param>
    /// <param name="action">导入处理委托。</param>
    /// <returns>导入结果。</returns>
    public static Result ReadFile<TItem>(Context context, AttachInfo file, Action<ImportRow<TItem>> action)
    {
        var path = Config.GetUploadPath(file.Path);
        if (!File.Exists(path))
            return Result.Error(Language.ImportFileNotExists);

        if (path.EndsWith(".txt"))
        {
            var items = GetImportColumns(context, file.BizId);
            var columns = items?.Select(context.Language.GetFieldName<TItem>).ToList();
            if (columns != null && columns.Count > 0)
                return ReadTextFile(context, path, columns, action);
        }

        return ReadExcelFile(context, path, action);
    }

    /// <summary>
    /// 获取导入栏位信息列表。
    /// </summary>
    /// <param name="context">上下文对象。</param>
    /// <param name="bizId">业务ID。</param>
    /// <returns>导入栏位信息列表。</returns>
    public static List<ColumnInfo> GetImportColumns(Context context, string bizId)
    {
        var import = CreateImport(new ImportContext { Context = context, BizId = bizId });
        if (import == null)
            return [];

        import.InitColumns();
        return import.Columns;
    }

    /// <summary>
    /// 异步执行后台任务。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="task">后台任务。</param>
    /// <returns>执行结果。</returns>
    public static Task<Result> ExecuteAsync(Database db, TaskInfo task)
    {
        var context = new ImportContext
        {
            Database = db,
            Context = db.Context,
            BizId = task.BizId
        };
        var import = CreateImport(context);
        if (import == null)
            return Result.ErrorAsync("The import method is not registered and cannot be executed!");

        return import.ExecuteAsync(task.File);
    }

    private static ImportBase CreateImport(ImportContext context)
    {
        if (context.IsDictionary)
            return new AutoImport(context);

        if (!Config.ImportTypes.TryGetValue(context.BizId, out Type type))
            return null;

        var scope = Config.ServiceProvider.CreateScope();
        if (scope.ServiceProvider.GetRequiredService(type) is ImportBase import)
        {
            import.ImportContext = context;
            import.SetServiceContext(context.Context);
            return import;
        }

        return Activator.CreateInstance(type, context) as ImportBase;
    }

    private static Result ReadTextFile<TItem>(Context context, string path, List<string> columns, Action<ImportRow<TItem>> action)
    {
        var errors = new Dictionary<int, string>();
        var lines = File.ReadAllLines(path);
        if (lines == null || lines.Length == 0)
            return Result.Error(CoreLanguage.TipDataRequired);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var items = line.Split('\t');
            if (items[0] == columns[0])
                continue;

            var item = new ImportRow<TItem>(context);
            for (int j = 0; j < columns.Count; j++)
            {
                item[columns[j]] = items.Length > j ? items[j] : "";
            }
            action?.Invoke(item);
            if (!string.IsNullOrWhiteSpace(item.ErrorMessage))
                errors.Add(i, item.ErrorMessage);
        }
        return ReadResult(context, errors);
    }

    private static Result ReadExcelFile<TItem>(Context context, string path, Action<ImportRow<TItem>> action)
    {
        var excel = ExcelFactory.Create(path);
        if (excel == null)
            return Result.Error(CoreLanguage.TipExcelFailed);

        var errors = new Dictionary<int, string>();
        var lines = excel.SheetToDictionaries(0, 2);
        if (lines == null || lines.Count == 0)
            return Result.Error(CoreLanguage.TipDataRequired);

        for (int i = 0; i < lines.Count; i++)
        {
            var item = new ImportRow<TItem>(context);
            foreach (var line in lines[i])
            {
                item[line.Key] = line.Value;
            }
            action?.Invoke(item);
            if (!string.IsNullOrWhiteSpace(item.ErrorMessage))
                errors.Add(i, item.ErrorMessage);
        }
        return ReadResult(context, errors);
    }

    private static Result ReadResult(Context context, Dictionary<int, string> errors)
    {
        if (errors.Count == 0)
            return Result.Success(CoreLanguage.TipValidSuccess);

        var error = string.Join(Environment.NewLine, errors.Select(e =>
        {
            var rowNo = context.Language[CoreLanguage.TipRowNo].Replace("{key}", $"{e.Key}");
            return $"{rowNo}{e.Value}";
        }));
        return Result.Error($"{context.Language[CoreLanguage.TipValidFailed]}{Environment.NewLine}{error}");
    }
}