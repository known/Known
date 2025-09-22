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
}

partial class AdminClient
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId) => Http.GetAsync<List<AttachInfo>>($"/Admin/GetFiles?bizId={bizId}");
    public Task<Result> DeleteFileAsync(AttachInfo info) => Http.PostAsync("/Admin/DeleteFile", info);
}

partial class AdminService
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Database.GetFilesAsync(bizId);
    }

    public async Task<Result> DeleteFileAsync(AttachInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.Path))
            return Result.Error(Language.TipFileNotExists);

        var oldFiles = new List<string>();
        await Database.DeleteFileAsync(info, oldFiles);
        AttachFile.DeleteFiles(oldFiles);
        return Result.Success(Language.DeleteSuccess);
    }
}