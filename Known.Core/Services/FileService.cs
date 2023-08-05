namespace Known.Core.Services;

class FileService : BaseService
{
    internal FileService(Context context) : base(context) { }

    //Public
    internal static void DeleteFiles(Database db, string bizId, List<string> oldFiles)
    {
        var files = FileRepository.GetFiles(db, bizId);
        DeleteFiles(db, files, oldFiles);
    }

    internal static SysFile SaveFile(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles, bool isThumb = false)
    {
        if (file == null)
            return null;

        DeleteFiles(db, bizId, bizType, oldFiles);
        return AddFile(db, file, bizId, bizType, "", isThumb);
    }

    internal static List<SysFile> AddFiles(Database db, List<AttachFile> files, string bizId, string bizType, bool isThumb = false)
    {
        if (files == null || files.Count == 0)
            return null;

        var sysFiles = new List<SysFile>();
        foreach (var item in files)
        {
            var file = AddFile(db, item, bizId, bizType, "", isThumb);
            sysFiles.Add(file);
        }
        return sysFiles;
    }

    //File
    internal PagingResult<SysFile> QueryFiles(PagingCriteria criteria)
    {
        return FileRepository.QueryFiles(Database, criteria);
    }

    internal Result DeleteFile(SysFile file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return Result.Error("文件不存在！");

        Database.Delete(file);
        AttachFile.DeleteFile(file.Path);
        return Result.Success("删除成功！");
    }

    internal ImportFormInfo GetImport(string bizId)
    {
        var task = TaskRepository.GetTaskByBizId(Database, bizId);
        return ImportHelper.GetImport(bizId, task);
    }

    internal byte[] GetImportRule(string bizId) => ImportHelper.GetImportRule(bizId);

    internal List<SysFile> GetFiles(string bizId)
    {
        if (string.IsNullOrWhiteSpace(bizId))
            return null;

        var bizIds = bizId.Split(';');
        if (bizIds.Length > 1)
            return FileRepository.GetFiles(Database, bizIds);

        var bizNos = bizId.Split('_');
        return bizNos.Length == 2
             ? FileRepository.GetFiles(Database, bizNos[0], bizNos[1])
             : FileRepository.GetFiles(Database, bizId);
    }

    internal bool HasFiles(string bizId) => FileRepository.HasFiles(Database, bizId);

    internal FileUrlInfo GetFileUrl(string bizId)
    {
        var files = GetFiles(bizId);
        if (files == null || files.Count == 0)
            return null;

        var file = files.FirstOrDefault();
        return file.FileUrl;
    }

    internal Result UploadFile(UploadFormInfo info)
    {
        ImportFormInfo form = Utils.MapTo<ImportFormInfo>(info.Model);
        SysFile sysFile = null;
        var user = CurrentUser;
        var file = GetAttachFile(info, user, "File", form);
        var result = Database.Transaction("上传", db =>
        {
            sysFile = AddFile(db, file, form.BizId, form.BizType, form.Type, Utils.ConvertTo<bool>(form.IsThumb));
            if (form.BizType == ImportHelper.BizType)
            {
                var task = ImportHelper.CreateTask(form);
                task.Target = sysFile.Id;
                db.Save(task);
            }
        });
        result.Data = sysFile;
        if (result.IsValid && form.BizType == ImportHelper.BizType)
            result.Message += "等待后台导入中...";
        return result;
    }

    internal Result UploadFiles(UploadFormInfo info)
    {
        ImportFormInfo form = Utils.MapTo<ImportFormInfo>(info.Model);
        var sysFiles = new List<SysFile>();
        var user = CurrentUser;
        var files = GetAttachFiles(info, user, "Files", form);
        var result = Database.Transaction("上传", db =>
        {
            sysFiles = AddFiles(db, files, form.BizId, form.BizType, Utils.ConvertTo<bool>(form.IsThumb));
        });
        result.Data = sysFiles;
        return result;
    }

    internal Result UploadFile(UploadInfo info, string type)
    {
        if (info == null || info.Data == null || info.Data.Length == 0)
            return Result.Success("", new { Errno = -1 });

        var user = CurrentUser;
        var attach = new AttachFile(info, user, "Upload", type);
        attach.Category1 = "Upload";
        attach.Category2 = type;
        var file = AddFile(Database, attach, "Upload", type, "", false);
        return Result.Success("", file.Url);
    }

    internal static void DeleteFiles(Database db, string bizId, string bizType, List<string> oldFiles)
    {
        var files = FileRepository.GetFiles(db, bizId, bizType);
        DeleteFiles(db, files, oldFiles);
    }

    private static void DeleteFiles(Database db, List<SysFile> files, List<string> oldFiles)
    {
        if (files == null || files.Count == 0)
            return;

        foreach (var item in files)
        {
            oldFiles.Add(item.Path);
            if (!string.IsNullOrWhiteSpace(item.ThumbPath))
                oldFiles.Add(item.ThumbPath);

            db.Delete(item);
        }
    }

    private static SysFile AddFile(Database db, AttachFile attach, string bizId, string bizType, string note, bool isThumb)
    {
        attach.Save(isThumb).Wait();
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
        db.Save(file);
        return file;
    }
}