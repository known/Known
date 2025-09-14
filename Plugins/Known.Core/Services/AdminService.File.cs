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
            return Result.Error(CoreLanguage.TipFileNotExists);

        var oldFiles = new List<string>();
        await Database.DeleteFileAsync(info.Id, oldFiles);
        AttachFile.DeleteFiles(oldFiles);
        return Result.Success(Language.DeleteSuccess);
    }
}