using Known.Entities;
using Known.Extensions;
using Known.Helpers;
using Known.Repositories;

namespace Known.Services;

class FileService(Context context) : ServiceBase(context)
{
    //Public
    internal async Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        var files = await FileRepository.GetFilesAsync(db, bizId);
        await DeleteFilesAsync(db, files, oldFiles);
    }

    internal async Task<SysFile> SaveFileAsync(Database db, AttachFile file, string bizId, string bizType, List<string> oldFiles)
    {
        if (file == null)
            return null;

        await DeleteFilesAsync(db, bizId, bizType, oldFiles);
        return await AddFileAsync(db, file, bizId, bizType, "");
    }

    internal async Task<List<SysFile>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
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
            return Result.Error(Language["Tip.FileNotExists"]);

        await Database.DeleteAsync(file);
        AttachFile.DeleteFile(file);
        return Result.Success(Language.Success(Language.Delete));
    }

    public async Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        var task = await SystemRepository.GetTaskByBizIdAsync(Database, bizId);
        return ImportHelper.GetImport(Context, bizId, task);
    }

    public async Task<byte[]> GetImportRuleAsync(string bizId)
    {
        if (bizId.StartsWith("Dictionary"))
        {
            var module = await Platform.Module.GetModuleAsync(Context.Current.Id);
            return await ImportHelper.GetDictionaryRuleAsync(Context, module);
        }

        return await ImportHelper.GetImportRuleAsync(Context, bizId);
    }

    public Task<List<SysFile>> GetFilesAsync(string bizId)
    {
        if (string.IsNullOrWhiteSpace(bizId))
            return Task.FromResult(new List<SysFile>());

        var bizIds = bizId.Split(';');
        if (bizIds.Length > 1)
            return FileRepository.GetFilesAsync(Database, bizIds);

        var bizNos = bizId.Split('_');
        return bizNos.Length == 2
             ? FileRepository.GetFilesAsync(Database, bizNos[0], bizNos[1])
             : FileRepository.GetFilesAsync(Database, bizId);
    }

    //internal Task<bool> HasFilesAsync(string bizId) => FileRepository.HasFilesAsync(Database, bizId);

    //public async Task<FileUrlInfo> GetFileUrlAsync(string bizId)
    //{
    //    var files = await GetFilesAsync(bizId);
    //    if (files == null || files.Count == 0)
    //        return null;

    //    var file = files.FirstOrDefault();
    //    return file.FileUrl;
    //}

    public async Task<Result> UploadFilesAsync<TModel>(UploadInfo<TModel> info)
    {
        var form = info.Model as ImportFormInfo;
        SysTask task = null;
        var sysFiles = new List<SysFile>();
        var user = CurrentUser;
        var files = info.Files.GetAttachFiles(user, "Upload", form);
        var result = await Database.TransactionAsync(Language.Upload, async db =>
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
                result.Message += Language["Import.FileImporting"];
            else if (task != null)
                result = await ImportHelper.ExecuteAsync(Database, task);
        }
        return result;
    }

    //public Task<Result> UploadImageAsync(UploadInfo info) => UploadFileAsync(info, "Image");
    //public Task<Result> UploadVideoAsync(UploadInfo info) => UploadFileAsync(info, "Video");

    //private async Task<Result> UploadFileAsync(UploadInfo info, string type)
    //{
    //    if (info == null || info.Data == null || info.Data.Length == 0)
    //        return Result.Success("", "");

    //    var user = CurrentUser;
    //    var attach = new AttachFile(info, user);
    //    var fileId = Utils.GetGuid();
    //    attach.IsWeb = true;
    //    attach.FilePath = $@"{user.CompNo}\{type}\{fileId}{attach.ExtName}";
    //    attach.Category1 = "WWW";
    //    attach.Category2 = type;
    //    var file = await AddFileAsync(Database, attach, "Upload", type, "");
    //    return Result.Success("", file.Url);
    //}

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
            Category1 = attach.Category1 ?? "File",
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