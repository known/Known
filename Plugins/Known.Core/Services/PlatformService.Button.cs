namespace Known.Services;

partial class PlatformService
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
        return Database.SaveConfigAsync(Constant.KeyButton, datas, true);
    }
}