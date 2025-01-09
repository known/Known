namespace Known;

/// <summary>
/// 定时任务摘要信息类。
/// </summary>
public class TaskSummaryInfo
{
    /// <summary>
    /// 取得或设置定时任务当前状态。
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置定时任务当前描述信息。
    /// </summary>
    public string Message { get; set; }
}