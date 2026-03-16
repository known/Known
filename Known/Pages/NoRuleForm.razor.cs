namespace Known.Pages;

/// <summary>
/// 编码规则表单页面组件类。
/// </summary>
public partial class NoRuleForm
{
    private void OnAdd()
    {
        Model.Data.Rules.Add(new NoRuleItem { Type = NoRuleType.Fixed });
        OnChange();
    }

    private void OnDelete(NoRuleItem row)
    {
        Model.Data.Rules.Remove(row);
        OnChange();
    }

    private void OnMoveUp(NoRuleItem row)
    {
        Model.Data.Rules.MoveRow(row, true);
        OnChange();
    }

    private void OnMoveDown(NoRuleItem row)
    {
        Model.Data.Rules.MoveRow(row, true);
        OnChange();
    }

    private void OnItemChange(NoRuleType type) => OnChange();
    private void OnItemChange(string value) => OnChange();
    private void OnChange() => Model.Data.Sample = Model.Data.GetMaxRuleNo(DateTime.Now, 0);
}

/// <summary>
/// 编码规则类型表单组件类。
/// </summary>
public class NoRuleTypeForm : AntForm<SysNoRule> { }

/// <summary>
/// 编码规则表格组件类。
/// </summary>
public class NoRuleItemTable : AntTable<NoRuleItem> { }

/// <summary>
/// 编码规则类型枚举选择组件类。
/// </summary>
public class NoRuleTypeSelect : AntSelectEnum<NoRuleType> { }