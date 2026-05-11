namespace Known.Plugins.Tables;

/// <summary>
/// 无代码表格设置表单组件类。
/// </summary>
public partial class NoCodeTableForm
{
    private KCodeView script;
    private string CodeScript => Generator.GetScript(Model.Data);
    private string ScriptFile => $"{Model.Data.Script}.sql";

    /// <inheritdoc />
    protected override void OnTabChange(string tab)
    {
        base.OnTabChange(tab);
        if (tab == nameof(CodeScript))
            script?.SetCode(CodeScript);
    }

    private async Task OnExecute()
    {
        if (string.IsNullOrWhiteSpace(Model.Data.Script))
        {
            UI.Error("实体名为空，不能执行脚本！");
            return;
        }

        var info = new AutoInfo<string> { PageId = Model.Data.Script, Data = CodeScript };
        var result = await CodeService.CreateTableAsync(info);
        UI.Result(result);
    }
}