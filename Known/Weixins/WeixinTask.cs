namespace Known.Weixins;

[Task(BizType)]
class WeixinTask : TaskBase
{
    private const string BizType = "WeixinTemplate";

    internal static SysTask CreateTask(WeixinTemplateInfo info)
    {
        return new SysTask
        {
            BizId = info.BizId,
            Type = BizType,
            Name = info.BizName,
            Target = Utils.ToJson(info.Template),
            Status = TaskJobStatus.Pending
        };
    }

    public override async Task<Result> ExecuteAsync(Database db, SysTask task)
    {
        var info = Utils.FromJson<TemplateInfo>(task.Target);
        if (info == null)
            return Result.Error("The template is null.");

        return await WeixinApi.SendTemplateMessageAsync(info);
    }
}