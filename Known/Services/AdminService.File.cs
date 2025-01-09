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
    /// <param name="file">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(AttachInfo file);

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

partial class AdminService
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Task.FromResult(new List<AttachInfo>());
    }

    public Task<Result> DeleteFileAsync(AttachInfo file)
    {
        return Result.SuccessAsync("删除成功！");
    }

    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<SysFile>());
    }

    public Task<Result> DeleteFilesAsync(List<SysFile> models)
    {
        return Result.SuccessAsync("删除成功！");
    }
}

partial class AdminClient
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Http.GetAsync<List<AttachInfo>>($"/Admin/GetFiles?bizId={bizId}");
    }

    public Task<Result> DeleteFileAsync(AttachInfo file)
    {
        return Http.PostAsync("/Admin/DeleteFile", file);
    }

    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<SysFile>("/Admin/QueryFiles", criteria);
    }

    public Task<Result> DeleteFilesAsync(List<SysFile> models)
    {
        return Http.PostAsync("/Admin/DeleteFiles", models);
    }
}