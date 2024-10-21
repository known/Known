namespace Known.Services;

/// <summary>
/// 系统模块服务接口。
/// </summary>
public interface IModuleService : IService
{
    /// <summary>
    /// 异步获取系统模块列表。
    /// </summary>
    /// <returns>系统模块列表。</returns>
    Task<List<SysModule>> GetModulesAsync();

    /// <summary>
    /// 异步获取系统模块信息。
    /// </summary>
    /// <param name="id">模块ID。</param>
    /// <returns>系统模块信息。</returns>
    Task<SysModule> GetModuleAsync(string id);

    /// <summary>
    /// 异步导出系统模块数据。
    /// </summary>
    /// <returns>导出文件对象。</returns>
    Task<FileDataInfo> ExportModulesAsync();

    /// <summary>
    /// 异步导入系统模块数据。
    /// </summary>
    /// <param name="info">导入文件。</param>
    /// <returns>导入结果。</returns>
    Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info);

    /// <summary>
    /// 异步删除系统模块。
    /// </summary>
    /// <param name="models">系统模块列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteModulesAsync(List<SysModule> models);

    /// <summary>
    /// 异步复制系统模块。
    /// </summary>
    /// <param name="models">系统模块列表。</param>
    /// <returns>复制结果。</returns>
    Task<Result> CopyModulesAsync(List<SysModule> models);

    /// <summary>
    /// 异步移动多条系统模块。
    /// </summary>
    /// <param name="models">系统模块列表。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModulesAsync(List<SysModule> models);

    /// <summary>
    /// 异步移动单条系统模块。
    /// </summary>
    /// <param name="model">系统模块信息。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModuleAsync(SysModule model);

    /// <summary>
    /// 异步保存系统模块。
    /// </summary>
    /// <param name="model">系统模块信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveModuleAsync(SysModule model);
}