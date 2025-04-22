namespace Known.Services;

/// <summary>
/// 代码生成服务接口。
/// </summary>
public interface ICodeService : IService
{
    /// <summary>
    /// 异步获取数据库表列表。
    /// </summary>
    /// <returns></returns>
    Task<List<CodeInfo>> GetDbTablesAsync();

    /// <summary>
    /// 异步获取数据库表字段列表。
    /// </summary>
    /// <param name="tableName">数据库表。</param>
    /// <returns></returns>
    Task<List<CodeFieldInfo>> GetDbFieldsAsync(string tableName);

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

    /// <summary>
    /// 异步保存代码。
    /// </summary>
    /// <param name="info">代码信息。</param>
    /// <returns>创建结果。</returns>
    Task<Result> SaveCodeAsync(AutoInfo<string> info);

    /// <summary>
    /// 异步创建数据库表。
    /// </summary>
    /// <param name="info">建表脚本对象。</param>
    /// <returns>创建结果。</returns>
    Task<Result> CreateTableAsync(AutoInfo<string> info);
}

[Client]
class CodeClient(HttpClient http) : ClientBase(http), ICodeService
{
    public Task<List<CodeInfo>> GetDbTablesAsync()
    {
        return Http.GetAsync<List<CodeInfo>>("/Code/GetDbTables");
    }

    public Task<List<CodeFieldInfo>> GetDbFieldsAsync(string tableName)
    {
        return Http.GetAsync<List<CodeFieldInfo>>($"/Code/GetDbFields?tableName={tableName}");
    }

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

    public Task<Result> SaveCodeAsync(AutoInfo<string> info)
    {
        return Http.PostAsync("/Code/SaveCode", info);
    }

    public Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        return Http.PostAsync("/Code/CreateTable", info);
    }
}

[Service]
class CodeService(Context context) : ServiceBase(context), ICodeService
{
    public Task<List<CodeInfo>> GetDbTablesAsync()
    {
        return Task.FromResult(new List<CodeInfo>());
    }

    public Task<List<CodeFieldInfo>> GetDbFieldsAsync(string tableName)
    {
        return Task.FromResult(new List<CodeFieldInfo>());
    }

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

    public Task<Result> SaveCodeAsync(AutoInfo<string> info)
    {
        return Result.SuccessAsync("保存成功！");
    }

    public Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        return Result.SuccessAsync("执行成功！");
    }
}