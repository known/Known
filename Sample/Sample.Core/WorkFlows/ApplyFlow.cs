namespace Sample.Core.WorkFlows;

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
    public override Result OnCommiting(Database db, FlowFormInfo info)
    {
        //表单提交前的校验
        var model = db.QueryById<TbApply>(info.BizId);
        var vr = model.Validate();
        if (!vr.IsValid)
            return vr;

        return base.OnCommiting(db, info);
    }
    //表单提交后
    public override void OnCommited(Database db, FlowFormInfo info) { }
    //表单撤回前
    public override Result OnRevoking(Database db, FlowFormInfo info) => Result.Success("");
    //表单撤回后
    public override void OnRevoked(Database db, FlowFormInfo info) { }
    //表单审核前
    public override Result OnVerifing(Database db, FlowFormInfo info) => Result.Success("");
    //表单审核后
    public override void OnVerified(Database db, FlowFormInfo info)
    {
        //表单审核通过操作
        if (info.FlowStatus == FlowStatus.Over)
        {
        }
    }
    //表单重启前
    public override Result OnRepeating(Database db, FlowFormInfo info) => Result.Success("");
    //表单重启后
    public override void OnRepeated(Database db, FlowFormInfo info) { }
    //流程停止前
    public override Result OnStopping(Database db, FlowFormInfo info) => Result.Success("");
    //流程停止后
    public override void OnStopped(Database db, FlowFormInfo info) { }
}