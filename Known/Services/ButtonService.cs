namespace Known.Services;

/// <summary>
/// 按钮服务接口。
/// </summary>
public interface IButtonService : IService
{
    /// <summary>
    /// 异步分页查询按钮信息列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取指定位置的按钮信息列表。
    /// </summary>
    /// <param name="position">按钮位置。</param>
    /// <returns>按钮信息列表。</returns>
    Task<List<ButtonInfo>> GetButtonsAsync(string position);

    /// <summary>
    /// 异步删除按钮信息列表。
    /// </summary>
    /// <param name="infos">按钮信息列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos);

    /// <summary>
    /// 异步保存按钮信息列表。
    /// </summary>
    /// <param name="info">按钮信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveButtonAsync(ButtonInfo info);
}

[Client]
class ButtonClient(HttpClient http) : ClientBase(http), IButtonService
{
    public Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria) => Http.QueryAsync<ButtonInfo>("/Button/QueryButtons", criteria);
    public Task<List<ButtonInfo>> GetButtonsAsync(string position) => Http.GetAsync<List<ButtonInfo>>($"/Button/GetButtons?position={position}");
    public Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos) => Http.PostAsync("/Button/DeleteButtons", infos);
    public Task<Result> SaveButtonAsync(ButtonInfo info) => Http.PostAsync("/Button/SaveButton", info);
}

[WebApi, Service]
class ButtonService(Context context) : ServiceBase(context), IButtonService
{
    public async Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria)
    {
        var datas = await Database.GetButtonsAsync();
        return datas.ToQueryResult(criteria);
    }

    public async Task<List<ButtonInfo>> GetButtonsAsync(string position)
    {
        var datas = await Database.GetButtonsAsync();
        return [.. datas.Where(b => b.Position?.Contains(position) == true)];
    }

    public async Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var datas = await Database.GetButtonsAsync();
        foreach (var info in infos)
        {
            var item = datas.FirstOrDefault(b => b.Id == info.Id);
            if (item != null)
                datas.Remove(item);
        }
        await SaveButtonsAsync(datas);
        return Result.Success(Language.DeleteSuccess);
    }

    public async Task<Result> SaveButtonAsync(ButtonInfo info)
    {
        var datas = await Database.GetButtonsAsync();
        var item = datas.FirstOrDefault(b => b.Id == info.Id);
        if (item == null)
        {
            item = new ButtonInfo();
            datas.Add(item);
        }
        item.Id = info.Id;
        item.Name = info.Name;
        item.Icon = info.Icon;
        item.Style = info.Style;
        item.Position = info.Position;
        await SaveButtonsAsync(datas);
        return Result.Success(Language.SaveSuccess);
    }

    private Task<Result> SaveButtonsAsync(List<ButtonInfo> datas)
    {
        return Database.SaveConfigAsync(Constants.KeyButton, datas, true);
    }
}