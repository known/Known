namespace Known.Clients;

public class FlowClient : ClientBase
{
    public FlowClient(Context context) : base(context) { }

    public Task<List<SysFlow>> GetFlowTodosAsync() => Context.GetAsync<List<SysFlow>>("Flow/GetFlowTodos");
    public Task<List<SysFlowLog>> GetFlowLogsAsync(string bizId) => Context.GetAsync<List<SysFlowLog>>($"Flow/GetFlowLogs?bizId={bizId}");
    public Task<Result> SubmitFlowAsync(FlowFormInfo info) => Context.PostAsync("Flow/SubmitFlow", info);
    public Task<Result> RevokeFlowAsync(FlowFormInfo info) => Context.PostAsync("Flow/RevokeFlow", info);
    public Task<Result> AssignFlowAsync(FlowFormInfo info) => Context.PostAsync("Flow/AssignFlow", info);
    public Task<Result> VerifyFlowAsync(FlowFormInfo info) => Context.PostAsync("Flow/VerifyFlow", info);
    public Task<Result> RepeatFlowAsync(FlowFormInfo info) => Context.PostAsync("Flow/RepeatFlow", info);
    public Task<Result> StopFlowAsync(FlowFormInfo info) => Context.PostAsync("Flow/StopFlow", info);
}