﻿namespace Known.Extensions;

static class PlatformExtension
{
    internal static async Task<List<ButtonInfo>> GetButtonsAsync(this Database db)
    {
        var datas = await db.GetConfigAsync<List<ButtonInfo>>(Constant.KeyButton, true);
        datas ??= [];
        foreach (var item in Config.Actions)
        {
            if (datas.Exists(d => d.Id == item.Id))
                continue;

            datas.Add(item.ToButton());
        }
        return datas;
    }
}