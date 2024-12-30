namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public interface IPlatformService : IService
{
    #region TopNav
    /// <summary>
    /// 异步获取顶部导航信息列表。
    /// </summary>
    /// <returns>顶部导航信息列表。</returns>
    Task<List<PluginInfo>> GetTopNavsAsync();
    
    /// <summary>
    /// 异步保存顶部导航信息列表。
    /// </summary>
    /// <param name="infos">顶部导航信息列表。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveTopNavsAsync(List<PluginInfo> infos);
    #endregion

    #region Button
    /// <summary>
    /// 异步分页查询按钮信息列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria);

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
    #endregion

    #region Menu
    /// <summary>
    /// 异步删除菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> DeleteMenuAsync(MenuInfo info);

    /// <summary>
    /// 异步保存菜单信息。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveMenuAsync(MenuInfo info);
    #endregion
}

class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
    #region TopNav
    public Task<List<PluginInfo>> GetTopNavsAsync()
    {
        return Task.FromResult(AppData.Data?.TopNavs);
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return AppData.SaveTopNavsAsync(infos);
    }
    #endregion

    #region Button
    public Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria)
    {
        var buttons = AppData.Data?.Buttons ?? [];
        if (buttons.Count == 0)
            buttons.AddRange(Config.Actions.Select(CreateButton));
        if (criteria.HasQuery(nameof(ButtonInfo.Name)))
        {
            var name = criteria.GetQueryValue(nameof(ButtonInfo.Name));
            buttons = buttons.Where(b => b.Name.Contains(name)).ToList();
        }
        var result = buttons.ToPagingResult(criteria);
        return Task.FromResult(result);
    }

    public Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos)
    {
        return AppData.DeleteButtonsAsync(infos);
    }

    public Task<Result> SaveButtonAsync(ButtonInfo info)
    {
        return AppData.SaveButtonAsync(info);
    }

    private ButtonInfo CreateButton(ActionInfo info)
    {
        return new ButtonInfo
        {
            Id = info.Id,
            Name = info.Name,
            Icon = info.Icon,
            Style = info.Style,
            Position = info.Position?.Split(',')
        };
    }
    #endregion

    #region Menu
    public Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        return AppData.DeleteMenuAsync(info);
    }

    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        return AppData.SaveMenuAsync(info);
    }
    #endregion
}

class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
    #region TopNav
    public Task<List<PluginInfo>> GetTopNavsAsync()
    {
        return Http.GetAsync<List<PluginInfo>>("/Platform/GetTopNavs");
    }

    public Task<Result> SaveTopNavsAsync(List<PluginInfo> infos)
    {
        return Http.PostAsync("/Platform/SaveTopNavs", infos);
    }
    #endregion

    #region Button
    public Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<ButtonInfo>("/Platform/QueryButtons", criteria);
    }

    public Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos)
    {
        return Http.PostAsync("/Platform/DeleteButtons", infos);
    }

    public Task<Result> SaveButtonAsync(ButtonInfo info)
    {
        return Http.PostAsync("/Platform/SaveButton", info);
    }
    #endregion

    #region Menu
    public Task<Result> DeleteMenuAsync(MenuInfo info)
    {
        return Http.PostAsync("/Platform/DeleteMenu", info);
    }

    public Task<Result> SaveMenuAsync(MenuInfo info)
    {
        return Http.PostAsync("/Platform/SaveMenu", info);
    }
    #endregion
}