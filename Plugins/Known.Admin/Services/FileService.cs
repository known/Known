namespace Known.Admin.Services;

/// <summary>
/// 系统附件服务接口。
/// </summary>
public interface IFileService : IService
{
    /// <summary>
    /// 异步分页查询系统附件。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除多条系统附件。
    /// </summary>
    /// <param name="models">系统附件列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFilesAsync(List<SysFile> models);
}

class FileService(Context context) : ServiceBase(context), IFileService
{
    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysFile.CreateTime)} desc"];
        return Database.QueryPageAsync<SysFile>(criteria);
    }

    public async Task<Result> DeleteFilesAsync(List<SysFile> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await DeleteFileAsync(db, item, oldFiles);
            }
        });
        if (result.IsValid)
            Platform.DeleteFiles(oldFiles);
        return result;
    }

    #region Static
    internal static async Task<List<AttachInfo>> GetFilesAsync(Database db, string bizId)
    {
        if (string.IsNullOrWhiteSpace(bizId))
            return [];

        List<AttachInfo> files = null;
        var bizIds = bizId.Split(';');
        if (bizIds.Length > 1)
        {
            files = await db.QueryListAsync<AttachInfo>(d => bizIds.Contains(d.BizId));
        }
        else if (!bizId.Contains('_'))
        {
            files = await db.QueryListAsync<AttachInfo>(d => d.BizId == bizId);
        }
        else
        {
            var bizId1 = bizId.Substring(0, bizId.IndexOf('_'));
            var bizType = bizId.Substring(bizId.IndexOf('_') + 1);
            files = await db.QueryListAsync<AttachInfo>(d => d.BizId == bizId1 && d.Type == bizType);
        }
        return files?.OrderBy(d => d.CreateTime).ToList();
    }

    internal static async Task<List<AttachInfo>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
    {
        if (files == null || files.Count == 0)
            return null;

        var sysFiles = new List<AttachInfo>();
        foreach (var item in files)
        {
            var file = await AddFileAsync(db, item, bizId, bizType, "");
            sysFiles.Add(file);
        }
        return sysFiles;
    }

    internal static async Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        var files = await db.QueryListAsync<SysFile>(d => d.BizId == bizId);
        await DeleteFilesAsync(db, files, oldFiles);
    }

    private static async Task DeleteFilesAsync(Database db, List<SysFile> files, List<string> oldFiles)
    {
        if (files == null || files.Count == 0)
            return;

        foreach (var item in files)
        {
            await DeleteFileAsync(db, item, oldFiles);
        }
    }

    private static async Task DeleteFileAsync(Database db, SysFile item, List<string> oldFiles)
    {
        oldFiles.Add(item.Path);
        if (!string.IsNullOrWhiteSpace(item.ThumbPath))
            oldFiles.Add(item.ThumbPath);

        await db.DeleteAsync(item);
    }

    private static async Task<AttachInfo> AddFileAsync(Database db, AttachFile attach, string bizId, string bizType, string note)
    {
        await attach.SaveAsync();
        attach.BizId = bizId;
        attach.BizType = bizType;
        var file = new SysFile
        {
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
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
        return Utils.MapTo<AttachInfo>(file);
    }
    #endregion
}