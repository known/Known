﻿namespace Known.Services;

public partial interface IPlatformService
{
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
}

partial class PlatformService
{
    public Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria)
    {
        var datas = AppData.Data.Buttons ?? [];
        if (datas.Count == 0)
            datas.AddRange(Config.Actions.Select(CreateButton));
        if (criteria.HasQuery(nameof(ButtonInfo.Name)))
        {
            var name = criteria.GetQueryValue(nameof(ButtonInfo.Name));
            datas = datas.Where(b => b.Name.Contains(name)).ToList();
        }
        var result = datas.ToPagingResult(criteria);
        return Task.FromResult(result);
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
        return Result.SuccessAsync(Language.Success(Language.Delete));
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
        return Result.SuccessAsync(Language.Success(Language.Save));
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
}

partial class PlatformClient
{
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
}