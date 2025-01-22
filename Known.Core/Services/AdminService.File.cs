namespace Known.Services;

partial class AdminService
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Database.GetFilesAsync(bizId);
    }

    public async Task<Result> DeleteFileAsync(AttachInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.Path))
            return Result.Error(Language["Tip.FileNotExists"]);

        await Database.DeleteFileAsync(info.Id);
        AttachFile.DeleteFile(info.Path);
        return Result.Success(Language.DeleteSuccess);
    }

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