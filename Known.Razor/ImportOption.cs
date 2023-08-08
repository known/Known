namespace Known.Razor;

public class ImportOption
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Template { get; set; }
    public ImportFormInfo Model { get; set; }
    public Func<List<Dictionary<string, string>>, Task<Result>> Action { get; set; }
    public Func<UploadFormInfo, Task<Result>> Upload { get; set; }
    public Action OnSuccess { get; set; }
}