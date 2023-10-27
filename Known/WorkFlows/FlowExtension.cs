namespace Known.WorkFlows;

public static class FlowExtension
{
    public static void BuildFlowLog(this RenderTreeBuilder builder, string bizId)
    {
        builder.FormList<FlowLogGrid>("流程记录", "flow-log", attr =>
        {
            attr.Set(c => c.BizId, bizId);
        });
    }
}