namespace Known.WorkFlows;

public abstract class BaseFlow(Context context)
{
    public Context Context { get; } = context;

    internal static BaseFlow Create(Context context, SysFlow flow)
    {
        if (!Config.FlowTypes.ContainsKey(flow.FlowCode))
            Check.Throw(context.Language["Tip.NotRegisterFlow"]);

        var type = Config.FlowTypes[flow.FlowCode];
        var instance = Activator.CreateInstance(type, context) as BaseFlow;
        return instance;
    }

    public virtual Task<Result> OnCommitingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");
    public virtual Task OnCommitedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;
    public virtual Task<Result> OnRevokingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");
    public virtual Task OnRevokedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;
    public virtual Task<Result> OnVerifingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");
    public virtual Task OnVerifiedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;
    public virtual Task<Result> OnRepeatingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");
    public virtual Task OnRepeatedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;
    public virtual Task<Result> OnStoppingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");
    public virtual Task OnStoppedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;
}