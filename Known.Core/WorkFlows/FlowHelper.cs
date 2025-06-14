namespace Known.WorkFlows;

class FlowHelper
{
    internal static FlowBase Create(Context context, SysFlow flow)
    {
        if (!Config.FlowTypes.TryGetValue(flow.FlowCode, out Type type))
            throw new SystemException(context.Language[CoreLanguage.TipNotRegisterFlow]);

        var scope = Config.ServiceProvider.CreateScope();
        if (scope.ServiceProvider.GetRequiredService(type) is FlowBase flowInstance)
        {
            flowInstance.Context = context;
            flowInstance.SetServiceContext(context);
            return flowInstance;
        }

        var instance = Activator.CreateInstance(type, context) as FlowBase;
        return instance;
    }
}