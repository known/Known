namespace Known.Demo.WorkFlows;

class ApplyFlow : BaseFlow
{
    private const string FlowCode = "ApplyFlow";
    private const string FlowName = "申请流程";

    internal static FlowBizInfo GetBizInfo(TbApply entity)
    {
        return new FlowBizInfo
        {
            FlowCode = FlowCode,
            FlowName = FlowName,
            BizId = entity.Id,
            BizName = entity.BizNo,
            BizUrl = "",
            BizStatus = FlowStatus.Save
        };
    }
    //表单提交前
    public override async Task<Result> OnCommitingAsync(Database db, FlowFormInfo info)
    {
        //表单提交前的校验
        var model = await db.QueryByIdAsync<TbApply>(info.BizId);
        var vr = model.Validate();
        if (!vr.IsValid)
            return vr;

        return await base.OnCommitingAsync(db, info);
    }
    //表单提交后
    public override Task OnCommitedAsync(Database db, FlowFormInfo info) => base.OnCommitedAsync(db, info);
    //表单撤回前
    public override Task<Result> OnRevokingAsync(Database db, FlowFormInfo info) => base.OnRevokingAsync(db, info);
    //表单撤回后
    public override Task OnRevokedAsync(Database db, FlowFormInfo info) => base.OnRevokedAsync(db, info);
    //表单审核前
    public override Task<Result> OnVerifingAsync(Database db, FlowFormInfo info) => base.OnVerifingAsync(db, info);
    //表单审核后
    public override Task OnVerifiedAsync(Database db, FlowFormInfo info)
    {
        //表单审核通过操作
        if (info.FlowStatus == FlowStatus.Over)
        {
        }
        return base.OnVerifiedAsync(db, info);
    }
    //表单重启前
    public override Task<Result> OnRepeatingAsync(Database db, FlowFormInfo info) => base.OnRepeatingAsync(db, info);
    //表单重启后
    public override Task OnRepeatedAsync(Database db, FlowFormInfo info) => base.OnRepeatedAsync(db, info);
    //流程停止前
    public override Task<Result> OnStoppingAsync(Database db, FlowFormInfo info) => base.OnStoppingAsync(db, info);
    //流程停止后
    public override Task OnStoppedAsync(Database db, FlowFormInfo info) => base.OnStoppedAsync(db, info);
}