namespace Known.Services;

partial class AdminService
{
    public Task<List<AttachInfo>> GetFilesAsync(string bizId)
    {
        return Database.GetFilesAsync(bizId);
    }

    public async Task<Result> DeleteFileAsync(AttachInfo file)
    {
        if (file == null || string.IsNullOrWhiteSpace(file.Path))
            return Result.Error(Language["Tip.FileNotExists"]);

        await Database.DeleteFileAsync(file.Id);
        AttachFile.DeleteFile(file.Path);
        return Result.Success(Language.Success(Language.Delete));
    }

    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysFile.CreateTime)} desc"];
        return Database.QueryPageAsync<SysFile>(criteria);
    }

    public async Task<Result> DeleteFilesAsync(List<SysFile> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteFileAsync(item, oldFiles);
            }
        });
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
        return result;
    }
}