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
        var import = CreateImport(context, bizId);
        if (import == null)
            return [];

        import.InitColumns();
        return import.Columns;
    }

    /// <summary>
    /// 异步执行后台任务。
    /// </summary>
    /// <param name="context">系统上下文。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="task">后台任务。</param>
    /// <returns>执行结果。</returns>
    public static Task<Result> ExecuteAsync(Context context, Database db, SysTask task)
    {
        var import = CreateImport(context, task.BizId, db);
        if (import == null)
            return Result.ErrorAsync("The import method is not registered and cannot be executed!");

        return import.ExecuteAsync(task.File);
    }

    private static ImportBase CreateImport(Context context, string bizId, Database db = null)
    {
        var impContext = new ImportContext(context) { BizId = bizId, Database = db };
        if (impContext.IsDictionary)
            return new AutoImport(impContext);

        if (!CoreConfig.ImportTypes.TryGetValue(impContext.BizType, out Type type))
            return null;

        var scope = Config.ServiceProvider.CreateScope();
        if (scope.ServiceProvider.GetRequiredService(type) is ImportBase import)
        {
            import.ImportContext = impContext;
            import.SetServiceContext(impContext.Context);
            return import;
        }

        return Activator.CreateInstance(type, context) as ImportBase;
    }

    private static Result ReadTextFile<TItem>(Context context, string path, List<string> columns, Action<ImportRow<TItem>> action)
    {
        var errors = new Dictionary<int, string>();
        var lines = File.ReadAllLines(path);
        if (lines == null || lines.Length == 0)
            return Result.Error(Language.TipDataRequired);

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
            return Result.Error(Language.TipExcelFailed);

        var errors = new Dictionary<int, string>();
        var lines = excel.SheetToDictionaries(0, 2);
        if (lines == null || lines.Count == 0)
            return Result.Error(Language.TipDataRequired);

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
            return Result.Success(Language.TipValidSuccess);

        var error = string.Join(Environment.NewLine, errors.Select(e =>
        {
            var rowNo = context.Language[Language.TipRowNo].Replace("{key}", $"{e.Key}");
            return $"{rowNo}{e.Value}";
        }));
        return Result.Error($"{context.Language[Language.TipValidFailed]}{Environment.NewLine}{error}");
    }
}

class AutoImport(ImportContext context) : ImportBase(context)
{
    public override async Task<Result> ExecuteAsync(AttachInfo file)
    {
        var database = Database;
        var param = await database.GetAutoPageAsync(ImportContext.PageId, ImportContext.PluginId);
        if (param == null)
            return Result.Error(Language.Required(Language.EntityPlugin));

        var tableName = param.Script;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required(Language.TableName));

        var fields = param.Form?.Fields;
        if (fields == null)
            return Result.Error(Language.Required(Language.Fields));

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

        return await database.TransactionAsync(Language.Import, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(tableName, item);
            }
        });
    }
}