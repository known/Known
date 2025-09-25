namespace Known.Services;

/// <summary>
/// 附件服务接口。
/// </summary>
public interface IFileService : IService
{
    /// <summary>
    /// 异步分页查询附件信息。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    Task<PagingResult<AttachInfo>> QueryFilesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除附件信息。
    /// </summary>
    /// <param name="infos">附件列表。</param>
    /// <returns></returns>
    Task<Result> DeleteFilesAsync(List<AttachInfo> infos);
}

[Client]
class FileClient(HttpClient http) : ClientBase(http), IFileService
{
    public Task<PagingResult<AttachInfo>> QueryFilesAsync(PagingCriteria criteria) => Http.QueryAsync<AttachInfo>("/File/QueryFiles", criteria);
    public Task<Result> DeleteFilesAsync(List<AttachInfo> infos) => Http.PostAsync("/File/DeleteFiles", infos);
}

[WebApi, Service]
class FileService(Context context) : ServiceBase(context), IFileService
{
    public Task<PagingResult<AttachInfo>> QueryFilesAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(AttachInfo.CreateTime)} desc"];
        return Database.Query<SysFile>(criteria).ToPageAsync<AttachInfo>();
    }

    public async Task<Result> DeleteFilesAsync(List<AttachInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteFileAsync(item, oldFiles);
            }
        });
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
        return result;
    }
}