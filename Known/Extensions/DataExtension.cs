namespace Known.Extensions;

/// <summary>
/// 数据访问扩展类。
/// </summary>
public static class DataExtension
{
    /// <summary>
    /// 异步创建数据表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="tableName">数据表名。</param>
    /// <param name="script">建表脚本。</param>
    /// <returns></returns>
    public static async Task<Result> CreateTableAsync(this Database db, string tableName, string script)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.TipTableRequired);

        try
        {
            try
            {
                var sql = $"select count(*) from {tableName}";
                var count = await db.ScalarAsync<int>(sql);
                if (count > 0)
                    return Result.Error(Language.TipTableHasData);

                sql = $"drop table {tableName}";
                await db.ExecuteAsync(sql);
            }
            catch
            {
            }

            await db.ExecuteAsync(script);
            return Result.Success(Language.ExecuteSuccess);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    /// <summary>
    /// 异步获取表单最大规则编号。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="db">数据库对象。</param>
    /// <param name="ruleCode">编码规则代码。</param>
    /// <param name="field">实体字段。</param>
    /// <param name="id">实体ID。</param>
    /// <returns></returns>
    public static async Task<string> GetMaxRuleNoAsync<T>(this Database db, string ruleCode, string field, string id = null)
    {
        if (string.IsNullOrWhiteSpace(ruleCode) || string.IsNullOrWhiteSpace(field))
            return string.Empty;

        var compNo = db.User?.CompNo;
        if (string.IsNullOrWhiteSpace(compNo))
            return string.Empty;

        var rule = await db.QueryAsync<SysNoRule>(d => d.CompNo == compNo && d.Code == ruleCode);
        if (rule?.Rules == null || rule.Rules.Count == 0)
            return string.Empty;

        var date = DateTime.Now;
        var serialIndex = rule.Rules.FindIndex(r => r.Type == NoRuleType.Serial);
        if (serialIndex < 0)
            return rule.GetMaxRuleNo(date, 0);

        var prefix = string.Join("", rule.Rules.Take(serialIndex).Select(r => r.Type switch
        {
            NoRuleType.Fixed => r.Value,
            NoRuleType.DateTime => date.ToString(r.Value),
            _ => string.Empty
        }));
        var suffix = string.Join("", rule.Rules.Skip(serialIndex + 1).Select(r => r.Type switch
        {
            NoRuleType.Fixed => r.Value,
            NoRuleType.DateTime => date.ToString(r.Value),
            _ => string.Empty
        }));

        var tableName = db.Provider.GetTableName(typeof(T));
        var fieldName = db.FormatName(field);
        var sql = $"select max({fieldName}) from {db.FormatName(tableName)} where {db.FormatName(nameof(EntityBase.CompNo))}=@CompNo";

        var parameters = new Dictionary<string, object>
        {
            [nameof(EntityBase.CompNo)] = compNo
        };

        if (!string.IsNullOrWhiteSpace(prefix))
        {
            sql += $" and {fieldName} like @Prefix";
            parameters["Prefix"] = $"{prefix}%";
        }

        if (!string.IsNullOrWhiteSpace(suffix))
        {
            sql += $" and {fieldName} like @Suffix";
            parameters["Suffix"] = $"%{suffix}";
        }

        if (!string.IsNullOrWhiteSpace(id))
        {
            sql += $" and {db.FormatName(nameof(EntityBase.Id))}<>@Id";
            parameters[nameof(EntityBase.Id)] = id;
        }

        var maxNo = await db.ScalarAsync<string>(sql, parameters);
        if (string.IsNullOrWhiteSpace(maxNo))
            return rule.GetMaxRuleNo(date, 0);

        var serialStart = prefix.Length;
        var serialLength = maxNo.Length - serialStart - suffix.Length;
        if (serialStart < 0 || serialLength <= 0 || serialStart + serialLength > maxNo.Length)
            return rule.GetMaxRuleNo(date, 0);

        var serialText = maxNo.Substring(serialStart, serialLength);
        var maxId = int.TryParse(serialText, out var value) ? value : 0;
        return rule.GetMaxRuleNo(date, maxId);
    }

    /// <summary>
    /// 根据ID获取实体插件参数配置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="moduleId">模块ID。</param>
    /// <param name="pluginId">插件ID。</param>
    /// <returns>实体插件参数配置信息。</returns>
    public static async Task<AutoPageInfo> GetAutoPageAsync(this Database db, string moduleId, string pluginId)
    {
        var entity = await db.QueryByIdAsync<SysModule>(moduleId);
        if (entity?.Target == nameof(ModuleType.Page)) //无代码配置插件
            return entity?.ToAutoPageInfo();

        var module = entity?.ToMenuInfo();
        if (module != null && module.Plugins != null && module.Plugins.Count > 0)
            return module.Plugins.GetPluginParameter<AutoPageInfo>(pluginId);

        return null;

        //var plugins = new List<PluginInfo>();
        //foreach (var item in AppData.Data.Modules)
        //{
        //    if (item.Plugins != null && item.Plugins.Count > 0)
        //        plugins.AddRange(item.Plugins);
        //}

        //var plugin = plugins.FirstOrDefault(p => p.Id == pluginId);
        //if (plugin == null)
        //    return null;

        //if (AppData.OnAutoPage != null)
        //    return AppData.OnAutoPage.Invoke(plugin);

        //return Utils.FromJson<AutoPageInfo>(plugin.Setting);
    }

    /// <summary>
    /// 异步设置数据权限。
    /// </summary>
    /// <typeparam name="T">数据权限类型。</typeparam>
    /// <param name="db">数据库对象。</param>
    /// <param name="userId">用户ID。</param>
    /// <returns></returns>
    public static async Task<T> GetDataPurviewAsync<T>(this Database db, string userId)
    {
        var user = await db.QueryByIdAsync<SysUser>(userId);
        if (user == null || string.IsNullOrWhiteSpace(user.Data))
            return default;

        var info = Utils.FromJson<T>(user.Data);
        if (info == null)
            return default;

        return info;
    }

    /// <summary>
    /// 异步添加操作日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="type">日志类型。</param>
    /// <param name="target">操作对象。</param>
    /// <param name="content">操作内容。</param>
    /// <returns></returns>
    public static Task AddLogAsync(this Database db, LogType type, string target, string content)
    {
        return db.AddLogAsync(new LogInfo { Type = type.ToString(), Target = target, Content = content });
    }

    internal static async Task<Database> GetDatabaseAsync(this Database database, AutoPageInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.Database) || info.Database == database.ConnectionName)
            return database;

        if (CoreConfig.OnDatabase == null)
            return database;

        return await CoreConfig.OnDatabase.Invoke(database, info);
    }
}