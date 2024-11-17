namespace Known.Services;

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
                await db.DeleteFileAsync(item, oldFiles);
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
            files = await db.Query<SysFile>().Where(d => bizIds.Contains(d.BizId)).ToListAsync<AttachInfo>();
        }
        else if (!bizId.Contains('_'))
        {
            files = await db.Query<SysFile>().Where(d => d.BizId == bizId).ToListAsync<AttachInfo>();
        }
        else
        {
            var bizId1 = bizId.Substring(0, bizId.IndexOf('_'));
            var bizType = bizId.Substring(bizId.IndexOf('_') + 1);
            files = await db.Query<SysFile>().Where(d => d.BizId == bizId1 && d.Type == bizType).ToListAsync<AttachInfo>();
        }
        return files?.OrderBy(d => d.CreateTime).ToList();
    }
    #endregion
}