namespace Known.Services;

class FileService : BaseService
{
    //Public
    internal static async Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        var files = await FileRepository.GetFilesAsync(db, bizId);
        await DeleteFilesAsync(db, files, oldFiles);
    }

    internal static async Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles)
    {
        if (file == null)
            return null;

        await DeleteFilesAsync(db, bizId, bizType, oldFiles);
        return await AddFileAsync(db, file, bizId, bizType, "");
    }

    internal static async Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
    {
        if (files == null || files.Count == 0)
            return null;

        var sysFiles = new List<SysFile>();
        foreach (var item in files)
        {
            var file = await AddFileAsync(db, item, bizId, bizType, "");
            sysFiles.Add(file);
        }
        return sysFiles;
    }

    //File
    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria)
    {
        return FileRepository.QueryFilesAsync(Database, criteria);
    }

    public async Task<Result> DeleteFileAsync(SysFile file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return Result.Error("文件不存在！");

        await Database.DeleteAsync(file);
        AttachFile.DeleteFile(file);
        return Result.Success("删除成功！");
    }

    public async Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        var task = await TaskRepository.GetTaskByBizIdAsync(Database, bizId);
        return ImportHelper.GetImport(bizId, task);
    }

    public Task<byte[]> GetImportRuleAsync(string bizId) => ImportHelper.GetImportRuleAsync(bizId);

    public Task<List<SysFile>> GetFilesAsync(string bizId)
    {
        if (string.IsNullOrWhiteSpace(bizId))
            return null;

        var bizIds = bizId.Split(';');
        if (bizIds.Length > 1)
            return FileRepository.GetFilesAsync(Database, bizIds);

        var bizNos = bizId.Split('_');
        return bizNos.Length == 2
             ? FileRepository.GetFilesAsync(Database, bizNos[0], bizNos[1])
             : FileRepository.GetFilesAsync(Database, bizId);
    }

    internal Task<bool> HasFilesAsync(string bizId) => FileRepository.HasFilesAsync(Database, bizId);

    public async Task<FileUrlInfo> GetFileUrlAsync(string bizId)
    {
        var files = await GetFilesAsync(bizId);
        if (files == null || files.Count == 0)
            return null;

        var file = files.FirstOrDefault();
        return file.FileUrl;
    }

    public async Task<Result> UploadFilesAsync(UploadFormInfo info)
    {
        ImportFormInfo form = Utils.MapTo<ImportFormInfo>(info.Model);
        SysTask task = null;
        var sysFiles = new List<SysFile>();
        var user = CurrentUser;
        var files = GetAttachFiles(info, user, "Upload", form);
        var result = await Database.TransactionAsync("上传", async db =>
        {
            sysFiles = await AddFilesAsync(db, files, form.BizId, form.BizType);
            if (form.BizType == ImportHelper.BizType)
            {
                task = ImportHelper.CreateTask(form);
                task.Target = sysFiles[0].Id;
                await db.SaveAsync(task);
            }
        });
        result.Data = sysFiles;
        if (result.IsValid && form.BizType == ImportHelper.BizType)
        {
            if (form.IsAsync)
                result.Message += "等待后台导入中...";
            else if (task != null)
                result = await TaskHelper.RunAsync(Database, task, ImportHelper.ExecuteAsync);
        }
        return result;
    }

    public Task<Result> UploadImageAsync(UploadInfo info) => UploadFileAsync(info, "Image");
    public Task<Result> UploadVideoAsync(UploadInfo info) => UploadFileAsync(info, "Video");

    private async Task<Result> UploadFileAsync(UploadInfo info, string type)
    {
        if (info == null || info.Data == null || info.Data.Length == 0)
            return Result.Success("", "");

        var user = CurrentUser;
        var attach = new AttachFile(info, user);
        var fileId = Utils.GetGuid();
        attach.IsWeb = true;
        attach.FilePath = $@"{user.CompNo}\{type}\{fileId}{attach.ExtName}";
        attach.Category1 = "WWW";
        attach.Category2 = type;
        var file = await AddFileAsync(Database, attach, "Upload", type, "");
        return Result.Success("", file.Url);
    }

    internal static async Task DeleteFilesAsync(Database db, string bizId, string bizType, List<string> oldFiles)
    {
        var files = await FileRepository.GetFilesAsync(db, bizId, bizType);
        await DeleteFilesAsync(db, files, oldFiles);
    }

    private static async Task DeleteFilesAsync(Database db, List<SysFile> files, List<string> oldFiles)
    {
        if (files == null || files.Count == 0)
            return;

        foreach (var item in files)
        {
            oldFiles.Add(item.Path);
            if (!string.IsNullOrWhiteSpace(item.ThumbPath))
                oldFiles.Add(item.ThumbPath);

            await db.DeleteAsync(item);
        }
    }

    private static async Task<SysFile> AddFileAsync(Database db, AttachFile attach, string bizId, string bizType, string note)
    {
        await attach.SaveAsync();
        attach.BizId = bizId;
        attach.BizType = bizType;
        var file = new SysFile
        {
            CompNo = attach.User.CompNo,
            AppId = attach.User.AppId,
            Category1 = attach.Category1 ?? "附件",
            Category2 = attach.Category2,
            Type = attach.BizType,
            BizId = attach.BizId,
            Name = attach.SourceName,
            Path = attach.FilePath,
            Size = attach.Size,
            SourceName = attach.SourceName,
            ExtName = attach.ExtName,
            ThumbPath = attach.ThumbPath,
            Note = note
        };
        await db.SaveAsync(file);
        return file;
    }
}