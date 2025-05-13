namespace Known.Components;

/// <summary>
/// 工具条组件类。
/// </summary>
public partial class KToolbar
{
    /// <summary>
    /// 取得或设置工具条模型。
    /// </summary>
    [Parameter] public ToolbarModel Model { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (Model == null)
            return;

        Model.OnRefresh = StateChanged;
        base.OnInitialized();
    }

    private void OnEditTool()
    {
        Plugin?.EditToolbar(this);
    }

    private DropdownModel GetDropdownModel(string textButton)
    {
        return new DropdownModel
        {
            TextButton = textButton,
            OnItemClick = e=>
            {
                Model.OnItemClick?.Invoke(e);
                return Task.CompletedTask;
            }
        };
    }
}