namespace Known.Helpers;

/// <summary>
/// 数据导入帮助者类。
/// </summary>
public sealed class ImportHelper
{
    internal const string BizType = "ImportFiles";

    private ImportHelper() { }

    internal static ImportFormInfo GetImport(Context context, string bizId, SysTask task)
    {
        var info = new ImportFormInfo { BizId = bizId, BizType = BizType, IsFinished = true };
        if (task != null)
        {
            switch (task.Status)
            {
                case SysTaskStatus.Pending:
                    info.Message = context.Language["Import.TaskPending"];
                    info.IsFinished = false;
                    break;
                case SysTaskStatus.Running:
                    info.Message = context.Language["Import.TaskRunning"];
                    info.IsFinished = false;
                    break;
                case SysTaskStatus.Failed:
                    info.Message = context.Language["Import.TaskFailed"];
                    info.Error = task.Note;
                    break;
                case SysTaskStatus.Success:
                    info.Message = "";
                    break;
            }
        }

        return info;
    }

    internal static async Task<byte[]> GetImportRuleAsync(Database db, string bizId)
    {
        if (bizId.StartsWith("Dictionary"))
        {
            var id = bizId.Split('_')[1];
            var module = await db.QueryByIdAsync<SysModule>(id);
            return GetImportRule(db.Context, module.Form.Fields);
        }

        var columns = GetImportColumns(db.Context, bizId);
        if (columns == null || columns.Count == 0)
            return [];

        var fields = columns.Select(c => new FormFieldInfo
        {
            Id = c.Id,
            Name = db.Context.Language.GetString(c),
            Required = c.Required,
            Length = c.GetImportRuleNote(db.Context)
        }).ToList();
        return GetImportRule(db.Context, fields);
    }

    private static List<ColumnInfo> GetImportColumns(Context context, string bizId)
    {
        var import = ImportBase.Create(new ImportContext { Context = context, BizId = bizId });
        if (import == null)
            return [];

        import.InitColumns();
        return import.Columns;
    }

    private static byte[] GetImportRule(Context context, List<FormFieldInfo> fields)
    {
        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        sheet.SetCellValue("A1", context.Language["Import.TemplateTips"], new StyleInfo { IsBorder = true });
        sheet.MergeCells(0, 0, 1, fields.Count);
        for (int i = 0; i < fields.Count; i++)
        {
            var field = fields[i];
            var note = !string.IsNullOrWhiteSpace(field.Length) ? $"{field.Length}" : "";
            sheet.SetColumnWidth(i, 13);
            sheet.SetCellValue(1, i, note, new StyleInfo { IsBorder = true, IsTextWrapped = true });
            var fontColor = field.Required ? Color.Red : Color.White;
            sheet.SetCellValue(2, i, field.Name, new StyleInfo { IsBorder = true, FontColor = fontColor, BackgroundColor = Utils.FromHtml("#6D87C1") });
        }
        sheet.SetRowHeight(1, 30);
        var stream = excel.SaveToStream();
        return stream.ToArray();
    }

    internal static SysTask CreateTask(ImportFormInfo form)
    {
        return new SysTask
        {
            BizId = form.BizId,
            Type = form.BizType,
            Name = form.BizName,
            Target = "",
            Status = SysTaskStatus.Pending
        };
    }

    internal static async Task<Result> ExecuteAsync(Database db, SysTask task)
    {
        var context = new ImportContext
        {
            Database = db,
            Context = db.Context,
            BizId = task.BizId
        };
        var import = ImportBase.Create(context);
        if (import == null)
            return Result.Error("The import method is not registered and cannot be executed!");

        var file = await db.QueryByIdAsync<SysFile>(task.Target);
        return await import.ExecuteAsync(file);
    }

    /// <summary>
    /// 异步执行数据导入定时任务。
    /// </summary>
    /// <returns></returns>
    public static Task ExecuteAsync() => TaskHelper.RunAsync(BizType, ExecuteAsync);

    /// <summary>
    /// 读取导入文件并处理导入逻辑。
    /// </summary>
    /// <typeparam name="TItem">导入数据类型。</typeparam>
    /// <param name="context">上下文对象。</param>
    /// <param name="file">导入文件对象。</param>
    /// <param name="action">导入处理委托。</param>
    /// <returns>导入结果。</returns>
    public static Result ReadFile<TItem>(Context context, SysFile file, Action<ImportRow<TItem>> action)
    {
        var path = Config.GetUploadPath(file.Path);
        if (!File.Exists(path))
            return Result.Error(context.Language["Import.FileNotExists"]);

        if (path.EndsWith(".txt"))
        {
            var items = GetImportColumns(context, file.BizId);
            var columns = items?.Select(context.Language.GetString).ToList();
            if (columns != null && columns.Count > 0)
                return ReadTextFile(context, path, columns, action);
        }

        return ReadExcelFile(context, path, action);
    }

    private static Result ReadTextFile<TItem>(Context context, string path, List<string> columns, Action<ImportRow<TItem>> action)
    {
        var errors = new Dictionary<int, string>();
        var lines = File.ReadAllLines(path);
        if (lines == null || lines.Length == 0)
            return Result.Error(context.Language["Import.DataRequired"]);

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
        //用File读取流，再创建Excel实例，适配Docker环境
        var bytes = File.ReadAllBytes(path);
        var stream = new MemoryStream(bytes);
        var excel = ExcelFactory.Create(stream);
        if (excel == null)
            return Result.Error(context.Language["Import.ExcelFailed"]);

        var errors = new Dictionary<int, string>();
        var lines = excel.SheetToDictionaries(0, 2);
        if (lines == null || lines.Count == 0)
            return Result.Error(context.Language["Import.DataRequired"]);

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
            return Result.Success(context.Language["Import.ValidSuccess"]);

        var error = string.Join(Environment.NewLine, errors.Select(e =>
        {
            var rowNo = context.Language["Import.RowNo"].Replace("{key}", $"{e.Key}");
            return $"{rowNo}{e.Value}";
        }));
        return Result.Error($"{context.Language["Import.ValidFailed"]}{Environment.NewLine}{error}");
    }
}