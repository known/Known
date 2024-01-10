﻿using Known.Blazor;
using Known.Extensions;

namespace Known.WorkFlows;

public static class FlowExtension
{
    internal static List<ItemModel> GetFlowStepItems(this FlowInfo info)
    {
        if (info == null || info.Steps == null || info.Steps.Count == 0)
            return null;

        return info.Steps.Select(s => new ItemModel(s.Name)).ToList();
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
        page.ViewForm(FormType.Submit, row);
    }

    public static void SubmitFlow<TItem>(this BasePage<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal(page.Language["Button.Submit"], rows, page.Platform.Flow.SubmitFlowAsync);
    }

    public static void RevokeFlow<TItem>(this BasePage<TItem> page, TItem row) where TItem : FlowEntity, new() => page.RevokeFlow([row]);

    public static void RevokeFlow<TItem>(this BasePage<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal(page.Language["Button.Revoke"], rows, page.Platform.Flow.RevokeFlowAsync);
    }

    public static void AssignFlow<TItem>(this BasePage<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal(page.Language["Button.Assign"], [row], page.Platform.Flow.AssignFlowAsync);
    }

    public static void VerifyFlow<TItem>(this BasePage<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ViewForm(FormType.Verify, row);
    }

    public static void StopFlow<TItem>(this BasePage<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal(page.Language["Button.Stop"], rows, page.Platform.Flow.StopFlowAsync);
    }

    public static void RepeatFlow<TItem>(this BasePage<TItem> page, List<TItem> rows) where TItem : FlowEntity, new()
    {
        page.ShowFlowModal(page.Language["Button.Restart"], rows, page.Platform.Flow.RepeatFlowAsync);
    }

    private static void ShowFlowModal<TItem>(this BasePage<TItem> page, string name, List<TItem> rows, Func<FlowFormInfo, Task<Result>> action) where TItem : FlowEntity, new()
    {
        var flow = new FlowFormModel(page.UI);
        flow.Data = new FlowFormInfo { BizId = string.Join(",", rows.Select(r => r.Id)) };
        if (name == page.Language["Button.Assign"])
        {
            //指派给、备注
            flow.AddUserColumn(page.Language["AssignTo"], "User");
            flow.AddNoteColumn();
        }
        else
        {
            flow.AddReasonColumn(name);
        }

        var title = page.Language["Title.FlowAction"].Replace("{action}", name);
        var model = new DialogModel
        {
            Title = title,
            Content = builder => page.UI.BuildForm(builder, flow)
        };
        model.OnOk = async () =>
        {
            if (!flow.Validate())
                return;

            var result = await action?.Invoke(flow.Data);
            page.UI.Result(result, async () =>
            {
                await model.OnClose?.Invoke();
                await page.RefreshAsync();
            });
        };
        page.UI.ShowDialog(model);
    }
    #endregion
}