using System.Drawing;
using Known.Cells;

namespace Known.Services;

partial class AdminService
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
                    info.Message = Language["Import.TaskPending"];
                    info.IsFinished = false;
                    break;
                case TaskJobStatus.Running:
                    info.Message = Language["Import.TaskRunning"];
                    info.IsFinished = false;
                    break;
                case TaskJobStatus.Failed:
                    info.Message = Language["Import.TaskFailed"];
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
        byte[] data = null;
        var db = Database;
        if (bizId.StartsWith(ImportContext.AutoBizIdPrefix))
        {
            var id = bizId.Split('_')[1];
            var param = await db.GetAutoPageParameterAsync(id, "");
            data = GetImportRule(db.Context, param?.Form?.Fields);
        }
        else
        {
            var columns = ImportHelper.GetImportColumns(db.Context, bizId);
            if (columns != null && columns.Count > 0)
            {
                //TODO：导入实体类型限定的栏位多语言
                var fields = columns.Select(c => new FormFieldInfo
                {
                    Id = c.Id,
                    Name = db.Context.Language.GetString(c),
                    Required = c.Required,
                    Length = GetImportRuleNote(db.Context, c)
                }).ToList();
                data = GetImportRule(db.Context, fields);
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
        var files = info.Files.GetAttachFiles(CurrentUser, "Upload", form);
        var result = await database.TransactionAsync(Language.Upload, async db =>
        {
            sysFiles = await db.AddFilesAsync(files, form.BizId, form.BizType);
            if (form.BizType == ImportHelper.BizType)
            {
                task = CreateTask(form);
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
                TaskHelper.NotifyRun(form.BizType);
                result.Message += Language["Import.FileImporting"];
            }
            else if (task != null)
            {
                result = await ImportHelper.ExecuteAsync(database, task);
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
        sheet.SetCellValue("A1", context.Language["Import.TemplateTips"], new StyleInfo { IsBorder = true });
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
            return context.Language["Import.TemplateFill"].Replace("{text}", $"{string.Join(",", codes.Select(c => c.Code))}");
        }

        return column.Note;
    }
}