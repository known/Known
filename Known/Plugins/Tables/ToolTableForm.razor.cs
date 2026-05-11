namespace Known.Plugins.Tables;

/// <summary>
/// 工具条表格弹窗表单组件类。
/// </summary>
public partial class ToolTableForm
{
    private IButtonService Service;
    private List<ButtonInfo> Buttons = [];

    /// <summary>
    /// 取得或设置无代码表格插件配置信息。
    /// </summary>
    [Parameter] public AutoPageInfo Model { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IButtonService>();

        Buttons = await Service.GetButtonsAsync("Toolbar");
        Items = Model.Page.Tools ?? [];
        foreach (var item in Items)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
                continue;

            var btn = Buttons.FirstOrDefault(b => b.Id == item.Id);
            item.Name = btn?.Name;
            item.Icon = btn?.Icon;
        }
    }

    internal override string Query => "Toolbar";
    internal override List<string> SelectedItems => Items?.Select(d => d.Id).ToList();

    internal override List<ActionInfo> GetSelectedRows(List<ButtonInfo> rows)
    {
        return [.. rows.Select(r => r.ToAction())];
    }
}