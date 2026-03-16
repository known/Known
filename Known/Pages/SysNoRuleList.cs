namespace Known.Pages;

/// <summary>
/// 编码规则页面组件类。
/// </summary>
[Route("/sys/no-rules")]
[Menu(Constants.System, "编码规则", "number", 2)]
public class SysNoRuleList : BaseTablePage<SysNoRule>
{
    private INoRuleService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<INoRuleService>();

        Table.SetAdminTable();
        Table.FormType = typeof(NoRuleForm);
        Table.Form = new FormInfo { Width = 800, SmallLabel = true };
        Table.OnQuery = Service.QueryNoRulesAsync;
    }

    /// <summary>
    /// 新增编码规则。
    /// </summary>
    [Action] public void New() => Table.NewForm(Service.SaveNoRuleAsync, new SysNoRule());

    /// <summary>
    /// 编辑编码规则。
    /// </summary>
    /// <param name="row"></param>
    [Action] public void Edit(SysNoRule row) => Table.EditForm(Service.SaveNoRuleAsync, row);
    
    /// <summary>
    /// 删除编码规则。
    /// </summary>
    /// <param name="row"></param>
    [Action] public void Delete(SysNoRule row) => Table.Delete(Service.DeleteNoRulesAsync, row);
    
    /// <summary>
    /// 批量删除编码规则。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteNoRulesAsync);
}