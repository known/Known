namespace Known.WorkFlows;

class FlowHelper
{
    internal static FlowBase Create(Context context, SysFlow flow)
    {
        if (!Config.FlowTypes.TryGetValue(flow.FlowCode, out Type type))
            throw new SystemException(context.Language[CoreLanguage.TipNotRegisterFlow]);
        var instance = Activator.CreateInstance(type, context) as FlowBase;
        return instance;
    }
}