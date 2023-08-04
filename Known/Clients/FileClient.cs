namespace Known.Clients;

public class FileClient : ClientBase
{
    public FileClient(Context context) : base(context) { }

    public Task<PagingResult<SysFile>> QueryFilesAsync(PagingCriteria criteria) => Context.QueryAsync<SysFile>("File/QueryFiles", criteria);
    public Task<ImportFormInfo> GetImportAsync(string bizId) => Context.GetAsync<ImportFormInfo>($"File/GetImport?bizId={bizId}");
    public Task<byte[]> GetImportRuleAsync(string bizId) => Context.GetAsync<byte[]>($"File/GetImportRule?bizId={bizId}");
    public Task<List<SysFile>> GetFilesAsync(string bizId) => Context.GetAsync<List<SysFile>>($"File/GetFiles?bizId={bizId}");
    public Task<FileUrlInfo> GetFileUrlAsync(string bizId) => Context.GetAsync<FileUrlInfo>($"File/GetFileUrl?bizId={bizId}");
    public Task<Result> DeleteFileAsync(SysFile file) => Context.PostAsync("File/DeleteFile", file);
    public Task<Result> UploadImageAsync(UploadInfo info) => Context.PostAsync("File/UploadImage", info);
    public Task<Result> UploadVideoAsync(UploadInfo info) => Context.PostAsync("File/UploadVideo", info);
    public Task<Result> UploadFileAsync(HttpContent content) => Context.PostAsync("File/UploadFile", content);
    public Task<Result> UploadFilesAsync(HttpContent content) => Context.PostAsync("File/UploadFiles", content);
}