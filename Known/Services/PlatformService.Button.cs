namespace Known.Services;

public partial interface IPlatformService
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

partial class PlatformService
{
    public Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria)
    {
        var datas = AppData.GetButtons();
        if (criteria.HasQuery(nameof(ButtonInfo.Name)))
        {
            var name = criteria.GetQueryValue(nameof(ButtonInfo.Name));
            datas = datas.Where(b => b.Name.Contains(name)).ToList();
        }
        var result = datas.ToPagingResult(criteria);
        return Task.FromResult(result);
    }

    public Task<List<ButtonInfo>> GetButtonsAsync(string position)
    {
        var datas = AppData.GetButtons();
        var infos = datas.Where(b => b.Position?.Contains(position) == true).ToList();
        return Task.FromResult(infos);
    }

    public Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        foreach (var info in infos)
        {
            var item = AppData.Data.Buttons.FirstOrDefault(b => b.Id == info.Id);
            if (item != null)
                AppData.Data.Buttons.Remove(item);
        }
        AppData.SaveData();
        return Result.SuccessAsync(Language.DeleteSuccess);
    }

    public Task<Result> SaveButtonAsync(ButtonInfo info)
    {
        var item = AppData.Data.Buttons.FirstOrDefault(b => b.Id == info.Id);
        if (item == null)
        {
            item = new ButtonInfo();
            AppData.Data.Buttons.Add(item);
        }
        item.Id = info.Id;
        item.Name = info.Name;
        item.Icon = info.Icon;
        item.Style = info.Style;
        item.Position = info.Position;
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess);
    }
}

partial class PlatformClient
{
    public Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<ButtonInfo>("/Platform/QueryButtons", criteria);
    }

    public Task<List<ButtonInfo>> GetButtonsAsync(string position)
    {
        return Http.GetAsync<List<ButtonInfo>>($"/Platform/GetButtons?position={position}");
    }

    public Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos)
    {
        return Http.PostAsync("/Platform/DeleteButtons", infos);
    }

    public Task<Result> SaveButtonAsync(ButtonInfo info)
    {
        return Http.PostAsync("/Platform/SaveButton", info);
    }
}