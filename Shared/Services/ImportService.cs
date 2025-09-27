namespace Known.Services;

/// <summary>
/// 导入服务接口。
/// </summary>
public interface IImportService : IService
{
    /// <summary>
    /// 异步获取导入表单数据信息。
    /// </summary>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>导入表单数据信息。</returns>
    Task<ImportFormInfo> GetImportAsync(string bizId);

    /// <summary>
    /// 异步获取数据导入规范文件。
    /// </summary>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>导入规范文件。</returns>
    Task<byte[]> GetImportRuleAsync(string bizId);

    /// <summary>
    /// 异步导入系统附件。
    /// </summary>
    /// <param name="info">系统附件信息。</param>
    /// <returns>导入结果。</returns>
    Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info);
}

[Client]
class ImportClient(HttpClient http) : ClientBase(http), IImportService
{
    public Task<ImportFormInfo> GetImportAsync(string bizId) => Http.GetAsync<ImportFormInfo>($"/Import/GetImport?bizId={bizId}");
    public Task<byte[]> GetImportRuleAsync(string bizId) => Http.GetAsync<byte[]>($"/Import/GetImportRule?bizId={bizId}");
    public Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info) => Http.PostAsync("/Import/ImportFiles", info);
}

[WebApi, Service]
class ImportService(Context context) : ServiceBase(context), IImportService
{
    public async Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        var db = Database;
        var task = await db.GetTaskAsync(bizId);
        var info = new ImportFormInfo { BizId = bizId, BizType = ImportHelper.BizType, IsFinished = true };
        if (task != null)
        {
            switch (task.Status)
            {
                case TaskJobStatus.Pending:
                    info.Message = Language[Language.ImportTaskPending];
                    info.IsFinished = false;
                    break;
                case TaskJobStatus.Running:
                    info.Message = Language[Language.ImportTaskRunning];
                    info.IsFinished = false;
                    break;
                case TaskJobStatus.Failed:
                    info.Message = Language[Language.ImportFailed];
                    info.Error = task.Note;
                    break;
                case TaskJobStatus.Success:
                    info.Message = "";
                    break;
            }
        }
        return info;
    }

    public async Task<byte[]> GetImportRuleAsync(string bizId)
    {
        byte[] data = [];
        var db = Database;
        if (bizId.StartsWith(Config.AutoBizIdPrefix))
        {
            var bizIds = bizId.Split('_');
            var pageId = bizIds.Length > 1 ? bizIds[1] : "";
            var pluginId = bizIds.Length > 2 ? bizIds[2] : "";
            var param = await db.GetAutoPageAsync(pageId, pluginId);
            data = GetImportRule(Context, param?.Form?.Fields);
        }
        else
        {
            var columns = ImportHelper.GetImportColumns(Context, bizId);
            if (columns != null && columns.Count > 0)
            {
                //TODO：导入实体类型限定的栏位多语言
                var fields = columns.Select(c => new FormFieldInfo
                {
                    Id = c.Id,
                    Name = Context.Language.GetString(c),
                    Required = c.Required,
                    Length = GetImportRuleNote(Context, c)
                }).ToList();
                data = GetImportRule(Context, fields);
            }
        }
        return data;
    }

    public async Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info)
    {
        TaskInfo task = null;
        var form = info.Model;
        var sysFiles = new List<AttachInfo>();
        var database = Database;
        var files = info.Files.GetAttachFiles("Upload", form);
        var result = await database.TransactionAsync(Language.Upload, async db =>
        {
            sysFiles = await db.AddFilesAsync(files, form.BizId);
            if (form.BizType == ImportHelper.BizType)
            {
                task = CreateTask(form);
                task.CreateBy = db.UserName;
                task.CreateTime = DateTime.Now;
                task.Target = sysFiles[0].Id;
                task.File = sysFiles[0];
                if (form.IsAsync)
                    await db.CreateTaskAsync(task);
            }
        });
        result.Data = sysFiles;
        if (result.IsValid && form.BizType == ImportHelper.BizType)
        {
            if (form.IsAsync)
            {
                task.File = sysFiles.First();
                TaskHelper.NotifyRun(task, Context);
                result.Message += Language[Language.ImportFileImporting];
            }
            else if (task != null)
            {
                result = await ImportHelper.ExecuteAsync(Context, database, task);
            }
        }
        return result;
    }

    private static TaskInfo CreateTask(ImportFormInfo form)
    {
        return new TaskInfo
        {
            BizId = form.BizId,
            Type = form.BizType,
            Name = form.BizName,
            Target = "",
            Status = TaskJobStatus.Pending
        };
    }

    private static byte[] GetImportRule(Context context, List<FormFieldInfo> fields)
    {
        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        sheet.SetCellValue("A1", context.Language[Language.TipTemplateTips], new StyleInfo { IsBorder = true });
        if (fields != null && fields.Count > 0)
        {
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
        }
        sheet.SetRowHeight(1, 30);
        var stream = excel.SaveToStream();
        return stream.ToArray();
    }

    private static string GetImportRuleNote(Context context, ColumnInfo column)
    {
        if (!string.IsNullOrWhiteSpace(column.Category))
        {
            var codes = Cache.GetCodes(column.Category);
            return context.Language[Language.TipTemplateFill].Replace("{text}", $"{string.Join(",", codes.Select(c => c.Code))}");
        }

        return column.Note;
    }
}