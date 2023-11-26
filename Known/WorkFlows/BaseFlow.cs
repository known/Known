using System.Reflection;

namespace Known.WorkFlows;

public abstract class BaseFlow
{
    private static readonly Dictionary<string, Type> flowTypes = [];
    public static void Register(Assembly assembly)
    {
        if (assembly == null)
            return;

        var types = assembly.GetTypes();
        if (types == null || types.Length == 0)
            return;

        foreach (var item in types)
        {
            if (item.IsSubclassOf(typeof(BaseFlow)))
                flowTypes[item.Name] = item;
        }
    }

    internal static BaseFlow Create(SysFlow flow)
    {
        if (!flowTypes.ContainsKey(flow.FlowCode))
            Check.Throw("流程未注册！");

        var type = flowTypes[flow.FlowCode];
        var instance = Activator.CreateInstance(type);
        return (BaseFlow)instance;
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