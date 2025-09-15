namespace Known.WorkFlows;

/// <summary>
/// 流程界面扩展方法类。
/// </summary>
public static class FlowUIExtension
{
    /// <summary>
    /// 获取工作流表格行操作按钮列表。
    /// </summary>
    /// <typeparam name="TItem">表格数据类型。</typeparam>
    /// <param name="table">表格配置模型。</param>
    /// <param name="row">表格行绑定的对象。</param>
    /// <returns>操作按钮列表。</returns>
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

    /// <summary>
    /// 判断流程是否可以提交。
    /// </summary>
    /// <param name="entity">流程实体对象。</param>
    /// <returns></returns>
    public static bool CanSubmit(this FlowEntity entity)
    {
        return entity.BizStatus == FlowStatus.Save ||
               entity.BizStatus == FlowStatus.Revoked ||
               entity.BizStatus == FlowStatus.VerifyFail ||
               entity.BizStatus == FlowStatus.Reapply;
    }

    /// <summary>
    /// 判断流程是否可以撤回。
    /// </summary>
    /// <param name="entity">流程实体对象。</param>
    /// <returns></returns>
    public static bool CanRevoke(this FlowEntity entity)
    {
        return entity.BizStatus == FlowStatus.Verifing;
    }

    #region FlowAction
    /// <summary>
    /// 弹出提交工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="page">页面组件对象。</param>
    /// <param name="row">表单绑定的对象。</param>
    public static void SubmitFlow<TItem>(this BasePage<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ViewForm(FormViewType.Submit, row);
    }

    /// <summary>
    /// 弹出提交工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="rows">流程数据对象列表。</param>
    public static Task SubmitFlowAsync<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        component.ShowFlowModal(component.Language["Button.Submit"], rows, component.Admin.SubmitFlowAsync);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 弹出撤回工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="row">流程数据对象。</param>
    public static Task RevokeFlowAsync<TItem>(this BaseComponent component, TItem row) where TItem : FlowEntity, new() => component.RevokeFlowAsync([row]);

    /// <summary>
    /// 弹出撤回工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="rows">流程数据对象列表。</param>
    public static Task RevokeFlowAsync<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        component.ShowFlowModal(component.Language["Button.Revoke"], rows, component.Admin.RevokeFlowAsync);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 弹出指派工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="row">流程数据对象。</param>
    public static Task AssignFlowAsync<TItem>(this BaseComponent component, TItem row) where TItem : FlowEntity, new()
    {
        component.ShowFlowModal(component.Language["Button.Assign"], [row], component.Admin.AssignFlowAsync);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 弹出审核工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="page">页面组件对象。</param>
    /// <param name="row">流程数据对象。</param>
    public static void VerifyFlow<TItem>(this BasePage<TItem> page, TItem row) where TItem : FlowEntity, new()
    {
        page.ViewForm(FormViewType.Verify, row);
    }

    /// <summary>
    /// 弹出停止工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="rows">流程数据对象列表。</param>
    public static Task StopFlowAsync<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        component.ShowFlowModal(component.Language["Button.Stop"], rows, component.Admin.StopFlowAsync);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 弹出重新申请工作流表单对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="component">组件对象。</param>
    /// <param name="rows">流程数据对象列表。</param>
    public static Task RepeatFlowAsync<TItem>(this BaseComponent component, List<TItem> rows) where TItem : FlowEntity, new()
    {
        component.ShowFlowModal(component.Language["Button.Restart"], rows, component.Admin.RepeatFlowAsync);
        return Task.CompletedTask;
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
            Content = builder => builder.Form(flow)
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