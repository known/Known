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
    /// 异步获取系统附件列表。
    /// </summary>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件列表。</returns>
    Task<List<SysFile>> GetFilesAsync(string bizId);

    /// <summary>
    /// 异步获取导入表单数据信息。
    /// </summary>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>导入表单数据信息。</returns>
    Task<ImportFormInfo> GetImportAsync(string bizId);

    /// <summary>
    /// 异步获取数据导入规范文件。
    /// </summary>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>导入规范文件。</returns>
    Task<byte[]> GetImportRuleAsync(string bizId);

    /// <summary>
    /// 异步删除多条系统附件。
    /// </summary>
    /// <param name="models">系统附件列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFilesAsync(List<SysFile> models);

    /// <summary>
    /// 异步删除单条系统附件。
    /// </summary>
    /// <param name="file">系统附件对象。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFileAsync(SysFile file);

    /// <summary>
    /// 异步导入系统附件。
    /// </summary>
    /// <param name="info">系统附件信息。</param>
    /// <returns>导入结果。</returns>
    Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info);
}