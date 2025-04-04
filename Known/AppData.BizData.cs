namespace Known;

public partial class AppData
{
    private static Dictionary<string, string> BizData { get; set; } = [];
    // 业务数据存储文件路径
    internal static string KdbPath { get; set; }

    internal static void LoadBizData()
    {
        if (!File.Exists(KdbPath))
            return;

        var bytes = File.ReadAllBytes(KdbPath);
        BizData = ParseData<Dictionary<string, string>>(bytes);
    }

    /// <summary>
    /// 分页查询业务数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="key">数据存储键。</param>
    /// <param name="criteria">查询条件。</param>
    /// <param name="filter">查询过滤。</param>
    /// <returns></returns>
    public static PagingResult<T> QueryModels<T>(string key, PagingCriteria criteria, Func<List<T>, List<T>> filter = null)
    {
        var datas = GetBizData<List<T>>(key) ?? [];
        if (filter != null)
            datas = filter.Invoke(datas);
        return datas == null ? new PagingResult<T>(0, []) : datas.ToPagingResult(criteria);
    }

    /// <summary>
    /// 根据ID获取业务数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="key">数据存储键。</param>
    /// <param name="id">ID。</param>
    /// <returns></returns>
    public static T GetModel<T>(string key, string id)
    {
        var idName = nameof(EntityBase.Id);
        var datas = GetBizData<List<T>>(key) ?? [];
        if (string.IsNullOrWhiteSpace(id))
            return datas.FirstOrDefault();

        return datas.FirstOrDefault(d => CheckIdValue(d, idName, id));
    }

    /// <summary>
    /// 删除业务数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="key">数据存储键。</param>
    /// <param name="infos">业务数据列表。</param>
    /// <returns></returns>
    public static Result DeleteModels<T>(string key, List<T> infos)
    {
        var idName = nameof(EntityBase.Id);
        var datas = GetBizData<List<T>>(key) ?? [];
        foreach (var info in infos)
        {
            var id = info.Property(idName)?.ToString();
            var item = datas.FirstOrDefault(d => CheckIdValue(d, idName, id));
            if (item != null)
                datas.Remove(item);
        }
        return SaveBizData(key, datas);
    }

    /// <summary>
    /// 保存业务数据。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="key">数据存储键。</param>
    /// <param name="info">业务数据。</param>
    /// <returns></returns>
    public static Result SaveModel<T>(string key, T info)
    {
        var idName = nameof(EntityBase.Id);
        var id = info.Property(idName)?.ToString();
        if (string.IsNullOrWhiteSpace(id))
            return Result.Error($"The model must have a property of {idName}.");

        var datas = GetBizData<List<T>>(key) ?? [];
        var item = datas.FirstOrDefault(d => CheckIdValue(d, idName, id));
        if (item != null)
            datas.Remove(item);
        datas.Add(info);
        return SaveBizData(key, datas);
    }

    internal static Result SaveBizData(string key, object value)
    {
        BizData[key] = Utils.ToJson(value);
        var bytes = FormatData(BizData);
        File.WriteAllBytes(KdbPath, bytes);
        return Result.Success("Save successful!");
    }

    internal static T GetBizData<T>(string key)
    {
        if (BizData.TryGetValue(key, out var value))
            return Utils.FromJson<T>(value);

        return default;
    }

    private static bool CheckIdValue(object item, string idName, string id)
    {
        var value = item.Property(idName)?.ToString();
        return value == id;
    }
}