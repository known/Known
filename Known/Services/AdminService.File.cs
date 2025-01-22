namespace Known.Services;

public partial interface IAdminService
{
    /// <summary>
    /// 异步获取系统附件列表。
    /// </summary>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件列表。</returns>
    Task<List<AttachInfo>> GetFilesAsync(string bizId);

    /// <summary>
    /// 异步删除单条系统附件。
    /// </summary>
    /// <param name="info">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(AttachInfo info);

    /// <summary>
    /// 异步分页查询系统附件。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<AttachInfo>> QueryFilesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除多条系统附件。
    /// </summary>
    /// <param name="infos">系统附件列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFilesAsync(List<AttachInfo> infos);
}

partial class AdminService
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Task.FromResult(new List<AttachInfo>());
    }

    public Task<Result> DeleteFileAsync(AttachInfo info)
    {
        return Result.SuccessAsync(Language.Success(Language.Delete));
    }

    public Task<PagingResult<AttachInfo>> QueryFilesAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<AttachInfo>());
    }

    public Task<Result> DeleteFilesAsync(List<AttachInfo> infos)
    {
        return Result.SuccessAsync(Language.Success(Language.Delete));
    }
}

partial class AdminClient
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Http.GetAsync<List<AttachInfo>>($"/Admin/GetFiles?bizId={bizId}");
    }

    public Task<Result> DeleteFileAsync(AttachInfo info)
    {
        return Http.PostAsync("/Admin/DeleteFile", info);
    }

    public Task<PagingResult<AttachInfo>> QueryFilesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<AttachInfo>("/Admin/QueryFiles", criteria);
    }

    public Task<Result> DeleteFilesAsync(List<AttachInfo> infos)
    {
        return Http.PostAsync("/Admin/DeleteFiles", infos);
    }
}