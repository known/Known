using Known.Services;
using Known.WorkFlows;

namespace Known.Designers;

class DataHelper
{
    internal static List<EntityInfo> Models = [];
    internal static List<FlowInfo> Flows = [];

    internal static async Task InitializeAsync(ModuleService service)
    {
        var modules = await service.GetModulesAsync();
        if (modules == null || modules.Count == 0)
            return;

        Models.Clear();
        var models = modules.Where(m => !string.IsNullOrWhiteSpace(m.EntityData) && m.EntityData.Contains('|')).Select(m => m.EntityData).ToList();
        foreach (var item in models)
        {
            var model = GetEntityInfo(item);
            Models.Add(model);
        }

        Flows.Clear();
        var flows = modules.Where(m => !string.IsNullOrWhiteSpace(m.FlowData) && m.FlowData.Contains('|')).Select(m => m.FlowData).ToList();
        foreach (var item in flows)
        {
            var flow = GetFlowInfo(item);
            Flows.Add(flow);
        }
    }

    #region Entity
    internal static EntityInfo GetEntity(string model)
    {
        var info = new EntityInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Models.FirstOrDefault(m => m.Id == model);

        return GetEntityInfo(model);
    }

    private static EntityInfo GetEntityInfo(string model)
    {
        var info = new EntityInfo();
        var lines = model.Split([.. Environment.NewLine])
                         .Where(s => !string.IsNullOrWhiteSpace(s))
                         .ToArray();

        if (lines.Length > 0)
        {
            var values = lines[0].Split('|');
            if (values.Length > 0) info.Name = values[0];
            if (values.Length > 1) info.Id = values[1];
            if (values.Length > 2) info.IsFlow = values[2] == "Y";
        }

        if (lines.Length > 1)
        {
            for (int i = 1; i < lines.Length; i++)
            {
                var field = new FieldInfo();
                var values = lines[i].Split('|');
                if (values.Length > 0) field.Name = values[0];
                if (values.Length > 1) field.Id = values[1];
                if (values.Length > 2) field.Type = Utils.ConvertTo<FieldType>(values[2]);
                if (values.Length > 3) field.Length = values[3];
                if (values.Length > 4) field.Required = values[4] == "Y";

                if (field.Type == FieldType.CheckBox || field.Type == FieldType.Switch)
                {
                    field.Length = "50";
                    field.Required = true;
                }

                info.Fields.Add(field);
            }
        }

        return info;
    }
    #endregion

    #region Flow
    internal static FlowInfo GetFlow(string model)
    {
        var info = new FlowInfo();
        if (string.IsNullOrWhiteSpace(model))
            return info;

        if (!model.Contains('|'))
            return Flows.FirstOrDefault(m => m.Id == model);

        return GetFlowInfo(model);
    }

    private static FlowInfo GetFlowInfo(string model)
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
                if (values.Length > 2) step.Type = values[2];
                if (values.Length > 3) step.User = values[3];
                if (values.Length > 4) step.Role = values[4];
                if (values.Length > 5) step.Pass = values[5];
                if (values.Length > 6) step.Fail = values[6];
                info.Steps.Add(step);
            }
        }

        return info;
    }
    #endregion
}