using Known.Cells;
using Known.Entities;

namespace Known.Helpers;

public sealed class ImportHelper
{
    internal const string BizType = "ImportFiles";

    private ImportHelper() { }

    internal static ImportFormInfo GetImport(string bizId, SysTask task)
    {
        var info = new ImportFormInfo { BizId = bizId, BizType = BizType, IsFinished = true };
        if (task != null)
        {
            switch (task.Status)
            {
                case TaskStatus.Pending:
                    info.Message = "导入任务等待中...";
                    info.IsFinished = false;
                    break;
                case TaskStatus.Running:
                    info.Message = "导入任务执行中...";
                    info.IsFinished = false;
                    break;
                case TaskStatus.Failed:
                    info.Message = "导入失败！";
                    info.Error = task.Note;
                    break;
                case TaskStatus.Success:
                    info.Message = "";
                    break;
            }
        }

        info.Columns = GetRuleColumns(bizId);
        return info;
    }

    private static List<string> GetRuleColumns(string bizId)
    {
        var columns = ImportBase.GetImportColumns(bizId);
        if (columns == null || columns.Count == 0)
            return new List<string>();

        return columns.Select(c => c.Name).ToList();
    }

    internal static Task<byte[]> GetImportRuleAsync(string bizId)
    {
        var columns = ImportBase.GetImportColumns(bizId);
        if (columns == null || columns.Count == 0)
            return Task.FromResult(Array.Empty<byte>());

        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        sheet.SetCellValue("A1", "提示：红色栏位为必填栏位！", new StyleInfo { IsBorder = true });
        sheet.MergeCells(0, 0, 1, columns.Count);
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            sheet.SetColumnWidth(i, 13);
            sheet.SetCellValue(1, i, column.Note, new StyleInfo { IsBorder = true, IsTextWrapped = true });
            var fontColor = column.Required ? System.Drawing.Color.Red : System.Drawing.Color.White;
            sheet.SetCellValue(2, i, column.Name, new StyleInfo { IsBorder = true, FontColor = fontColor, BackgroundColor = Utils.FromHtml("#6D87C1") });
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
        var import = ImportBase.Create(task.BizId, db);
        if (import == null)
            return Result.Error("导入方法未注册，无法执行！");

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

    public static Result ReadFile(SysFile file, Action<ImportRow> action)
    {
        var path = Config.GetUploadPath(file.Path);
        if (!File.Exists(path))
            return Result.Error("导入文件不存在！");

        if (!path.EndsWith(".txt"))
            return ReadExcelFile(path, action);

        var columns = string.IsNullOrWhiteSpace(file.Note)
                    ? new List<string>()
                    : ImportFormInfo.GetImportColumns(file.Note);
        return ReadTextFile(path, columns, action);
    }

    private static Result ReadExcelFile(string path, Action<ImportRow> action)
    {
        var excel = ExcelFactory.Create(path);
        if (excel == null)
            return Result.Error("Excel创建失败！");

        var errors = new Dictionary<int, string>();
        var lines = excel.SheetToDictionaries(0, 2);
        if (lines == null || lines.Count == 0)
            return Result.Error("导入数据不能为空！");

        for (int i = 0; i < lines.Count; i++)
        {
            var item = new ImportRow();
            foreach (var line in lines[i])
            {
                item[line.Key] = line.Value;
            }
            action?.Invoke(item);
            if (!string.IsNullOrWhiteSpace(item.ErrorMessage))
                errors.Add(i, item.ErrorMessage);
        }
        return ReadResult(errors);
    }

    private static Result ReadTextFile(string path, List<string> columns, Action<ImportRow> action)
    {
        var errors = new Dictionary<int, string>();
        var lines = File.ReadAllLines(path);
        if (lines == null || lines.Length == 0)
            return Result.Error("导入数据不能为空！");

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var items = line.Split('\t');
            if (items[0] == columns[0])
                continue;

            var item = new ImportRow();
            for (int j = 0; j < columns.Count; j++)
            {
                item[columns[j]] = items.Length > j ? items[j] : "";
            }
            action?.Invoke(item);
            if (!string.IsNullOrWhiteSpace(item.ErrorMessage))
                errors.Add(i, item.ErrorMessage);
        }
        return ReadResult(errors);
    }

    private static Result ReadResult(Dictionary<int, string> errors)
    {
        if (errors.Count == 0)
            return Result.Success("校验成功！");

        var error = string.Join(Environment.NewLine, errors.Select(e => $"第{e.Key}行：{e.Value}"));
        return Result.Error($"校验失败！{Environment.NewLine}{error}");
    }
}