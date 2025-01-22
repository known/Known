namespace Known.Services;

public partial interface IAdminService
{
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
    /// 异步导入系统附件。
    /// </summary>
    /// <param name="info">系统附件信息。</param>
    /// <returns>导入结果。</returns>
    Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info);
}

partial class AdminService
{
    public Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        return Task.FromResult(new ImportFormInfo());
    }

    public Task<byte[]> GetImportRuleAsync(string bizId)
    {
        return Task.FromResult(Array.Empty<byte>());
    }

    public Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info)
    {
        return Result.SuccessAsync(Language.Success(Language.Import));
    }
}

partial class AdminClient
{
    public Task<ImportFormInfo> GetImportAsync(string bizId)
    {
        return Http.GetAsync<ImportFormInfo>($"/Admin/GetImport?bizId={bizId}");
    }

    public Task<byte[]> GetImportRuleAsync(string bizId)
    {
        return Http.GetAsync<byte[]>($"/Admin/GetImportRule?bizId={bizId}");
    }

    public Task<Result> ImportFilesAsync(UploadInfo<ImportFormInfo> info)
    {
        return Http.PostAsync("/Admin/ImportFiles", info);
    }
}