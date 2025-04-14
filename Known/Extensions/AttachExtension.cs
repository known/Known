namespace Known.Extensions;

/// <summary>
/// 附件扩展类。
/// </summary>
public static class AttachExtension
{
    /// <summary>
    /// 将附件数据转换成附件类的实例。
    /// </summary>
    /// <param name="file">附件信息。</param>
    /// <param name="form">附件表单信息。</param>
    /// <returns></returns>
    public static AttachFile ToAttachFile(this FileDataInfo file, FileFormInfo form)
    {
        return new AttachFile(file, form.BizType, form.BizPath) { Category2 = form.Category };
    }

    /// <summary>
    /// 获取附件字段的文件对象列表。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="key">字段名。</param>
    /// <param name="bizType">业务类型。</param>
    /// <param name="bizPath">业务路径。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, string key, string bizType, string bizPath = null)
    {
        return files?.GetAttachFiles(key, new FileFormInfo { BizType = bizType, BizPath = bizPath });
    }

    /// <summary>
    /// 获取附件字段的文件对象列表。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="key">字段名。</param>
    /// <param name="form">附件表单对象。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, string key, FileFormInfo form)
    {
        if (files == null || files.Count == 0)
            return null;

        if (!files.TryGetValue(key, out List<FileDataInfo> value))
            return null;

        var attaches = new List<AttachFile>();
        foreach (var item in value)
        {
            var attach = item.ToAttachFile(form);
            attaches.Add(attach);
        }
        return attaches;
    }

    /// <summary>
    /// 将附件数据转换成附件类的实例，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="file">附件信息。</param>
    /// <param name="user">当前用户信息。</param>
    /// <param name="form">附件表单信息。</param>
    /// <returns></returns>
    public static AttachFile ToAttachFile(this FileDataInfo file, UserInfo user, FileFormInfo form)
    {
        return file.ToAttachFile(form);
    }

    /// <summary>
    /// 获取附件字段的文件对象列表，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="key">字段名。</param>
    /// <param name="bizType">业务类型。</param>
    /// <param name="bizPath">业务路径。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, string bizType, string bizPath = null)
    {
        return files?.GetAttachFiles(key, bizType, bizPath);
    }

    /// <summary>
    /// 获取附件字段的文件对象列表，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="key">字段名。</param>
    /// <param name="form">附件表单对象。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, FileFormInfo form)
    {
        return files?.GetAttachFiles(key, form);
    }
}