namespace Known.WorkFlows;

class FlowHelper
{
    // 取得流程模型列表。
    internal static List<FlowInfo> Flows { get; } = [];

    // 将流程配置转换成流程信息对象。
    internal static FlowInfo ToFlow(string model)
    {
        var info = new FlowInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Flows.FirstOrDefault(m => m.Id == model);

        return GetFlowInfo(model);
    }

    internal static FlowInfo GetFlowInfo(string model)
    {
        var info = new FlowInfo();
        var lines = model.Split([.. Environment.NewLine])
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0) info.Name = values[0];
            if (values.Length > 1) info.Id = values[1];
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var step = new FlowStepInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0) step.Name = values[0];
                if (values.Length > 1) step.Id = values[1];
                if (values.Length > 2) step.User = values[2];
                if (values.Length > 3) step.Role = values[3];
                if (values.Length > 4) step.Pass = values[4];
                if (values.Length > 5) step.Fail = values[5];
                info.Steps.Add(step);
            }
        }

        return info;
    }

    internal static FlowBase Create(Context context, SysFlow flow)
    {
        if (!CoreConfig.FlowTypes.TryGetValue(flow.FlowCode, out Type type))
            throw new SystemException(context.Language[FlowLanguage.TipNotRegisterFlow]);

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