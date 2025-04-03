namespace Known.Services;

partial class PlatformService
{
    public async Task<PagingResult<ButtonInfo>> QueryButtonsAsync(PagingCriteria criteria)
    {
        var datas = await GetButtonsAsync();
        return datas.ToQueryResult(criteria);
    }

    public async Task<List<ButtonInfo>> GetButtonsAsync(string position)
    {
        var datas = await GetButtonsAsync();
        return datas.Where(b => b.Position?.Contains(position) == true).ToList();
    }

    public async Task<Result> DeleteButtonsAsync(List<ButtonInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var datas = await GetButtonsAsync();
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
        var datas = await GetButtonsAsync();
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

    private async Task<List<ButtonInfo>> GetButtonsAsync()
    {
        var datas = await Database.GetConfigAsync<List<ButtonInfo>>(Constant.KeyButton, true);
        datas ??= [];
        foreach (var item in Config.Actions)
        {
            if (datas.Exists(d => d.Id == item.Id))
                continue;

            datas.Add(item.ToButton());
        }
        return datas;
    }

    private Task<Result> SaveButtonsAsync(List<ButtonInfo> datas)
    {
        return Database.SaveConfigAsync(Constant.KeyButton, datas, true);
    }
}