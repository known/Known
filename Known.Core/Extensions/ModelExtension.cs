namespace Known.Core.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region User
    /// <summary>
    /// 发送站内消息。
    /// </summary>
    /// <param name="user">用户对象。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="toUser">站内收件人。</param>
    /// <param name="subject">消息主题。</param>
    /// <param name="content">消息内容。</param>
    /// <param name="isUrgent">是否紧急。</param>
    /// <param name="filePath">附件路径。</param>
    /// <param name="bizId">关联业务数据ID。</param>
    /// <returns></returns>
    public static Task SendMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, bool isUrgent = false, string filePath = null, string bizId = null)
    {
        var level = isUrgent ? Constant.UMLUrgent : Constant.UMLGeneral;
        return SendMessageAsync(db, user, level, toUser, subject, content, filePath, bizId);
    }

    private static Task SendMessageAsync(Database db, UserInfo user, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = Constant.UMTypeReceive,
            MsgBy = user.Name,
            MsgLevel = level,
            Subject = subject,
            Content = content,
            FilePath = filePath,
            IsHtml = true,
            Status = Constant.UMStatusUnread,
            BizId = bizId
        };
        return db.SaveAsync(model);
    }
    #endregion

    #region File
    /// <summary>
    /// 将附件数据转换成附件类的实例。
    /// </summary>
    /// <param name="file">附件信息。</param>
    /// <param name="user">当前用户信息。</param>
    /// <param name="form">附件表单信息。</param>
    /// <returns></returns>
    public static AttachFile ToAttachFile(this FileDataInfo file, UserInfo user, FileFormInfo form)
    {
        return new AttachFile(file, user, form.BizType, form.BizPath) { Category2 = form.Category };
    }

    /// <summary>
    /// 获取附件字段的文件对象列表。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="key">字段名。</param>
    /// <param name="bizType">业务类型。</param>
    /// <param name="bizPath">业务路径。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, string bizType, string bizPath = null) => files?.GetAttachFiles(user, key, new FileFormInfo { BizType = bizType, BizPath = bizPath });

    internal static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, FileFormInfo form)
    {
        if (files == null || files.Count == 0)
            return null;

        if (!files.TryGetValue(key, out List<FileDataInfo> value))
            return null;

        var attaches = new List<AttachFile>();
        foreach (var item in value)
        {
            var attach = item.ToAttachFile(user, form);
            attaches.Add(attach);
        }
        return attaches;
    }
    #endregion

    #region Module
    internal static List<MenuInfo> ToMenus(this List<SysModule> modules, bool isAdmin)
    {
        if (modules == null || modules.Count == 0)
            return [];

        return modules.Select(m => new MenuInfo(m, isAdmin)).ToList();
    }

    internal static void RemoveModule(this List<SysModule> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }
    #endregion
}