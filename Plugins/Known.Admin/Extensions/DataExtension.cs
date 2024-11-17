namespace Known.Extensions;

static class DataExtension
{
    internal static async Task InitializeTableAsync(this Database db)
    {
        db.EnableLog = false;
        var exists = await db.ExistsAsync<SysModule>();
        if (!exists)
        {
            Console.WriteLine("Table is initializing...");
            var name = db.DatabaseType.ToString();
            foreach (var item in Config.Assemblies)
            {
                var script = Utils.GetResource(item, $"{name}.sql");
                if (string.IsNullOrWhiteSpace(script))
                    continue;

                await db.ExecuteAsync(script);
            }
            Console.WriteLine("Table is initialized.");
        }
    }

    internal static async Task<string> GetConfigAsync(this Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    internal static async Task<SystemInfo> GetSystemAsync(this Database db)
    {
        var json = await db.GetConfigAsync(Constants.KeySystem);
        return Utils.FromJson<SystemInfo>(json);
    }

    internal static async Task<Result> SaveCompanyDataAsync(this Database db, string compNo, object model)
    {
        var lang = db.Context.Language;
        var data = await db.QueryAsync<SysCompany>(d => d.Code == compNo);
        if (data == null)
            return Result.Error(lang["Tip.CompanyNotExists"]);

        data.SystemData = Utils.ToJson(model);
        await db.SaveAsync(data);
        return Result.Success(lang.Success(lang.Save));
    }

    internal static async Task CreateTaskAsync(this Database db, TaskInfo info)
    {
        var task = new SysTask();
        if (!string.IsNullOrWhiteSpace(info.Id))
            task.Id = info.Id;
        task.BizId = info.BizId;
        task.Type = info.Type;
        task.Name = info.Name;
        task.Target = info.Target;
        task.Status = info.Status;
        await db.SaveAsync(task);
    }

    internal static async Task SaveTaskAsync(this Database db, TaskInfo info)
    {
        var task = await db.QueryByIdAsync<SysTask>(info.Id);
        if (task == null)
            return;

        task.Status = info.Status;
        task.BeginTime = info.BeginTime;
        task.EndTime = info.EndTime;
        task.Note = info.Note;
        await db.SaveAsync(task);
    }
}