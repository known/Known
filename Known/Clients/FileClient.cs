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
    public Task<Result> UploadFileAsync(HttpContent content) => Context.PostAsync("File/UploadFile", content);
    public Task<Result> UploadFilesAsync(HttpContent content) => Context.PostAsync("File/UploadFiles", content);
    public Task<Result> UploadImageAsync(HttpContent content) => Context.PostAsync("File/UploadImage", content);
    public Task<Result> UploadVideoAsync(HttpContent content) => Context.PostAsync("File/UploadVideo", content);
}