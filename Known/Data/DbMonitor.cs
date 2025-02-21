namespace Known.Data;

/// <summary>
/// 数据库操作信息类。
/// </summary>
public class DbOperateInfo
{
    /// <summary>
    /// 取得或设置数据库操作类型（Insert/Update/Delete）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置数据库操作命令信息。
    /// </summary>
    public CommandInfo Command { get; set; }

    /// <summary>
    /// 取得或设置数据操作是否成功。
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 取得或设置数据实体类型。
    /// </summary>
    public Type ModelType { get; set; }

    /// <summary>
    /// 取得或设置数据库操作表名称。
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 取得或设置数据库操作参数。
    /// </summary>
    public Dictionary<string, object> Params { get; set; }

    /// <summary>
    /// 取得或设置数据库更新操作前实体表数据。
    /// </summary>
    public Dictionary<string, object> Original { get; set; }

    /// <summary>
    /// 取得或设置数据库删除操作前实体表数据。
    /// </summary>
    public List<Dictionary<string, object>> DeleteItems { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Operate:{Type}，{TableName}，{IsSuccess}");
        sb.Append($"，Params={Params?.Count}");
        sb.Append($"，Original={Original?.Count}");
        sb.Append($"，DeleteItems={DeleteItems?.Count}");
        return sb.ToString();
    }
}

class DbMonitor
{
    internal static void OnSql(CommandInfo info)
    {
        if (DatabaseOption.Instance.SqlMonitor == null)
            return;

        Task.Run(() => DatabaseOption.Instance.SqlMonitor.Invoke(info));
    }

    internal static void OnOperate(CommandInfo info, bool success)
    {
        if (!DatabaseOption.Instance.HasOperateMonitor)
            return;

        Task.Run(() =>
        {
            var operate = new DbOperateInfo
            {
                Type = info.Text.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase) 
                     ? "Insert" 
                     : info.Text.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase)
                     ? "Update" 
                     : "Delete",
                IsSuccess = success,
                Command = info,
                ModelType = info.Type,
                TableName = info.TableName,
                Params = info.Params,
                Original = info.Original,
                DeleteItems = info.DeleteItems
            };
            foreach (var item in DatabaseOption.Instance.OperateMonitors)
            {
                item.Invoke(operate);
            }
        });
    }
}