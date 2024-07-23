namespace Known.WorkFlows;

public static class FlowExtension
{
    internal static List<ItemModel> GetFlowStepItems(this FlowInfo info)
    {
        if (info == null || info.Steps == null || info.Steps.Count == 0)
            return null;

        return info.Steps.Select(s => new ItemModel(s.Id, s.Name)).ToList();
    }

    public static List<ActionInfo> GetFlowRowActions<TItem>(this TableModel<TItem> table, TItem row) where TItem : FlowEntity, new()
    {
        var actions = new List<ActionInfo>();
        foreach (var item in table.Actions)
        {
            if (item.Id == "Submit" && row.BizStatus == FlowStatus.Verifing)
                continue;
            if (item.Id == "Revoke" && row.BizStatus != FlowStatus.Verifing)
                continue;
            actions.Add(item);
        }
        return actions;
    }

    #region FlowAction
    public static void SubmitFlow<TItem>(this BasePage<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ViewForm(FormViewType.Submit, row);
    }

    public static async void SubmitFlow<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        var service = await component.CreateServiceAsync<IFlowService>();
        component.ShowFlowModal(component.Language["Button.Submit"], rows, service.SubmitFlowAsync);
    }

    public static void RevokeFlow<TItem>(this BaseComponent component, TItem row) where TItem : FlowEntity, new() => component.RevokeFlow([row]);

    public static async void RevokeFlow<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        var service = await component.CreateServiceAsync<IFlowService>();
        component.ShowFlowModal(component.Language["Button.Revoke"], rows, service.RevokeFlowAsync);
    }

    public static async void AssignFlow<TItem>(this BaseComponent component, TItem row) where TItem : FlowEntity, new()
    {
        var service = await component.CreateServiceAsync<IFlowService>();
        component.ShowFlowModal(component.Language["Button.Assign"], [row], service.AssignFlowAsync);
    }

    public static void VerifyFlow<TItem>(this BasePage<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ViewForm(FormViewType.Verify, row);
    }

    public static async void StopFlow<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        var service = await component.CreateServiceAsync<IFlowService>();
        component.ShowFlowModal(component.Language["Button.Stop"], rows, service.StopFlowAsync);
    }

    public static async void RepeatFlow<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        var service = await component.CreateServiceAsync<IFlowService>();
        component.ShowFlowModal(component.Language["Button.Restart"], rows, service.RepeatFlowAsync);
    }

    private static void ShowFlowModal<TItem>(this BaseComponent component, string name, List<TItem> rows, Func<FlowFormInfo, Task<Result>> action) where TItem : FlowEntity, new()
    {
        var flow = new FlowFormModel(component);
        flow.Data = new FlowFormInfo { BizId = string.Join(",", rows.Select(r => r.Id)) };
        if (name == component.Language["Button.Assign"])
        {
            flow.AddUserColumn("AssignTo", "User");
            flow.AddNoteColumn();
        }
        else
        {
            flow.AddReasonColumn(name);
        }

        var title = component.Language["Title.FlowAction"].Replace("{action}", name);
        var model = new DialogModel
        {
            Title = title,
            Content = builder => component.UI.BuildForm(builder, flow)
        };
        model.OnOk = async () =>
        {
            if (!flow.Validate())
                return;

            var result = await action?.Invoke(flow.Data);
            component.UI.Result(result, async () =>
            {
                await model.CloseAsync();
                await component.RefreshAsync();
            });
        };
        component.UI.ShowDialog(model);
    }
    #endregion
}