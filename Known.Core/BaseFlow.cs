using System.Reflection;

namespace Known.Core;

public abstract class BaseFlow
{
    private static readonly Dictionary<string, Type> flowTypes = new();
    public static void Register(Assembly assembly)
    {
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

    public virtual Result OnCommiting(Database db, FlowFormInfo info) => Result.Success("");
    public virtual void OnCommited(Database db, FlowFormInfo info) { }
    public virtual Result OnRevoking(Database db, FlowFormInfo info) => Result.Success("");
    public virtual void OnRevoked(Database db, FlowFormInfo info) { }
    public virtual Result OnVerifing(Database db, FlowFormInfo info) => Result.Success("");
    public virtual void OnVerified(Database db, FlowFormInfo info) { }
    public virtual Result OnRepeating(Database db, FlowFormInfo info) => Result.Success("");
    public virtual void OnRepeated(Database db, FlowFormInfo info) { }
    public virtual Result OnStopping(Database db, FlowFormInfo info) => Result.Success("");
    public virtual void OnStopped(Database db, FlowFormInfo info) { }
}