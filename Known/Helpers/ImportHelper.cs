namespace Known.Helpers;

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
                case TaskStatus.Pending:
                    info.Message = context.Language["Import.TaskPending"];
                    info.IsFinished = false;
                    break;
                case TaskStatus.Running:
                    info.Message = context.Language["Import.TaskRunning"];
                    info.IsFinished = false;
                    break;
                case TaskStatus.Failed:
                    info.Message = context.Language["Import.TaskFailed"];
                    info.Error = task.Note;
                    break;
                case TaskStatus.Success:
                    info.Message = "";
                    break;
            }
        }

        return info;
    }

    internal static Task<byte[]> GetDictionaryRuleAsync(Context context, SysModule module)
    {
        var fields = module.Form.Fields;
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
            var fontColor = field.Required ? System.Drawing.Color.Red : System.Drawing.Color.White;
            sheet.SetCellValue(2, i, field.Name, new StyleInfo { IsBorder = true, FontColor = fontColor, BackgroundColor = Utils.FromHtml("#6D87C1") });
        }
        sheet.SetRowHeight(1, 30);
        var stream = excel.SaveToStream();
        return Task.FromResult(stream.ToArray());
    }

    internal static Task<byte[]> GetImportRuleAsync(Context context, string bizId)
    {
        var import = ImportBase.Create(new ImportContext { Context = context, BizId = bizId });
        if (import == null)
            return Task.FromResult(Array.Empty<byte>());

        import.InitColumns();
        if (import.Columns == null || import.Columns.Count == 0)
            return Task.FromResult(Array.Empty<byte>());

        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        sheet.SetCellValue("A1", context.Language["Import.TemplateTips"], new StyleInfo { IsBorder = true });
        sheet.MergeCells(0, 0, 1, import.Columns.Count);
        for (int i = 0; i < import.Columns.Count; i++)
        {
            var column = import.Columns[i];
            sheet.SetColumnWidth(i, 13);
            var note = column.GetImportRuleNote(context);
            sheet.SetCellValue(1, i, note, new StyleInfo { IsBorder = true, IsTextWrapped = true });
            var fontColor = column.Required ? System.Drawing.Color.Red : System.Drawing.Color.White;
            var columnName = context.Language.GetString(column);
            sheet.SetCellValue(2, i, columnName, new StyleInfo { IsBorder = true, FontColor = fontColor, BackgroundColor = Utils.FromHtml("#6D87C1") });
        }
        sheet.SetRowHeight(1, 30);
        var stream = excel.SaveToStream();
        return Task.FromResult(stream.ToArray());
    }

    internal static SysTask CreateTask(ImportFormInfo form)
    {
        return new SysTask
        {
            BizId = form.BizId,
            Type = form.BizType,
            Name = form.BizName,
            Target = "",
            Status = TaskStatus.Pending
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

    public static Task ExecuteAsync()
    {
        return Task.Run(async () =>
        {
            await TaskHelper.RunAsync(BizType, ExecuteAsync);
        });
    }

    public static Result ReadFile<TItem>(Context context, SysFile file, Action<ImportRow<TItem>> action)
    {
        var path = Config.GetUploadPath(file.Path);
        if (!File.Exists(path))
            return Result.Error(context.Language["Import.FileNotExists"]);

        if (!path.EndsWith(".txt"))
            return ReadExcelFile<TItem>(context, path, action);

        var columns = string.IsNullOrWhiteSpace(file.Note)
                    ? []
                    : ImportFormInfo.GetImportColumns(file.Note);
        return ReadTextFile<TItem>(context, path, columns, action);
    }

    private static Result ReadExcelFile<TItem>(Context context, string path, Action<ImportRow<TItem>> action)
    {
        var excel = ExcelFactory.Create(path);
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