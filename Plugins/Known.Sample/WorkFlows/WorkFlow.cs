namespace Known.Sample.WorkFlows;

class WorkFlow(Context context) : FlowBase(context)
{
    private const string FlowCode = "WorkFlow";
    private const string FlowName = "工单流程";

    internal static async Task CreateAsync(Database db, TbWork model)
    {
        var info = new FlowBizInfo
        {
            FlowCode = FlowCode,
            FlowName = FlowName,
            BizId = model.Id,
            BizName = model.WorkNo,
            BizUrl = "",
            BizStatus = WorkStatus.Pending
        };
        await db.CreateFlowAsync(info);
    }
}