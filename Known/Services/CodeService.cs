namespace Known.Services;

/// <summary>
/// 代码生成服务接口。
/// </summary>
public interface ICodeService : IService
{
    /// <summary>
    /// 异步获取代码模型列表。
    /// </summary>
    /// <returns></returns>
    Task<List<CodeModelInfo>> GetModelsAsync();

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <param name="infos">删除对象列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteModelsAsync(List<CodeInfo> infos);

    /// <summary>
    /// 异步保存代码模型信息。
    /// </summary>
    /// <param name="info">代码模型信息。</param>
    /// <returns></returns>
    Task<Result> SaveModelAsync(CodeModelInfo info);
}

[Client]
class CodeClient(HttpClient http) : ClientBase(http), ICodeService
{
    public Task<List<CodeModelInfo>> GetModelsAsync()
    {
        return Http.GetAsync<List<CodeModelInfo>>("/Code/GetModels");
    }

    public Task<Result> DeleteModelsAsync(List<CodeInfo> infos)
    {
        return Http.PostAsync("/Code/DeleteModels", infos);
    }

    public Task<Result> SaveModelAsync(CodeModelInfo info)
    {
        return Http.PostAsync("/Code/SaveModel", info);
    }
}

[Service]
class CodeService(Context context) : ServiceBase(context), ICodeService
{
    public Task<List<CodeModelInfo>> GetModelsAsync()
    {
        var infos = AppData.LoadCodeModels();
        return Task.FromResult(infos);
    }

    public Task<Result> DeleteModelsAsync(List<CodeInfo> infos)
    {
        AppData.DeleteCodeModels(infos);
        return Result.SuccessAsync("删除成功！");
    }

    public Task<Result> SaveModelAsync(CodeModelInfo info)
    {
        AppData.SaveCodeModel(info);
        return Result.SuccessAsync("保存成功！", info);
    }
}