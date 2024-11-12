namespace Known.Extensions;

/// <summary>
/// 平台服务扩展类。
/// </summary>
public static class PlatformExtension
{
    /// <summary>
    /// 物理删除系统附件。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="filePaths">附件路径列表。</param>
    public static void DeleteFiles(this IPlatformService platform, List<string> filePaths)
    {
        filePaths.ForEach(AttachFile.DeleteFile);
    }
}