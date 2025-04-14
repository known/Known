namespace Known.Extensions;

/// <summary>
/// 附件数据扩展类。
/// </summary>
public static class FileExtension
{
    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件信息列表。</returns>
    public static async Task<List<AttachInfo>> GetFilesAsync(this Database db, string bizId)
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

    /// <summary>
    /// 异步添加系统附件信息，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="files">表单附件列表。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <returns>系统附件信息列表。</returns>
    public static Task<List<AttachInfo>> AddFilesAsync(this Database db, List<AttachFile> files, string bizId, string bizType)
    {
        return db.AddFilesAsync(files, bizId, key => { });
    }

    /// <summary>
    /// 异步添加系统附件信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="files">表单附件列表。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="action">附件业务类型。</param>
    /// <returns>系统附件信息列表。</returns>
    public static async Task<List<AttachInfo>> AddFilesAsync(this Database db, List<AttachFile> files, string bizId, Action<string> action = null)
    {
        if (files == null || files.Count == 0)
            return null;

        var sysFiles = new List<AttachInfo>();
        foreach (var item in files)
        {
            var file = await AddFileAsync(db, item, bizId);
            sysFiles.Add(file);
        }
        action?.Invoke($"{bizId}_{files[0].BizType}");
        return sysFiles;
    }

    /// <summary>
    /// 异步删除系统附件实体。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">附件ID。</param>
    /// <returns></returns>
    public static Task DeleteFileAsync(this Database db, string id)
    {
        return db.DeleteAsync<SysFile>(id);
    }

    /// <summary>
    /// 异步删除系统附件表数据。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="oldFiles">要物理删除的附件路径列表。</param>
    /// <returns></returns>
    public static async Task DeleteFilesAsync(this Database db, string bizId, List<string> oldFiles)
    {
        var files = await db.QueryListAsync<SysFile>(d => d.BizId == bizId);
        if (files == null || files.Count == 0)
            return;

        foreach (var item in files)
        {
            await db.DeleteFileAsync(item, oldFiles);
        }
    }

    internal static async Task DeleteFileAsync(this Database db, SysFile item, List<string> oldFiles)
    {
        oldFiles.Add(item.Path);
        if (!string.IsNullOrWhiteSpace(item.ThumbPath))
            oldFiles.Add(item.ThumbPath);

        await db.DeleteAsync(item);
    }

    internal static async Task DeleteFileAsync(this Database db, AttachInfo item, List<string> oldFiles)
    {
        oldFiles.Add(item.Path);
        if (!string.IsNullOrWhiteSpace(item.ThumbPath))
            oldFiles.Add(item.ThumbPath);

        await db.DeleteFileAsync(item.Id);
    }

    private static async Task<AttachInfo> AddFileAsync(Database db, AttachFile attach, string bizId, string note = null)
    {
        attach.FilePath = Path.Combine(db.User.CompNo, attach.FilePath);
        attach.BizId = bizId;
        await attach.SaveAsync();
        var file = new SysFile
        {
            CompNo = db.User.CompNo,
            AppId = db.User.AppId,
            Category1 = attach.Category1 ?? "File",
            Category2 = attach.Category2,
            Type = attach.BizType,
            BizId = attach.BizId,
            Name = attach.SourceName,
            Path = attach.FilePath,
            Size = attach.Size,
            SourceName = attach.SourceName,
            ExtName = attach.ExtName,
            ThumbPath = attach.ThumbPath,
            Note = note
        };
        await db.SaveAsync(file);
        return Utils.MapTo<AttachInfo>(file);
    }
}