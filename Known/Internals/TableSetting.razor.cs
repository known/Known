using AntDesign;

namespace Known.Internals;

/// <summary>
/// 表格列设置组件类。
/// </summary>
/// <typeparam name="TItem">表格行数据类型。</typeparam>
public partial class TableSetting<TItem>
{
    private List<ColumnInfo> columns = [];
    private ColumnInfo dragging;
    private readonly Trigger[] Triggers = [Trigger.Click];

    private List<ColumnInfo> LeftColumns => [.. columns.Where(c => c.Fixed == "left")];
    private List<ColumnInfo> RightColumns => [.. columns.Where(c => c.Fixed == "right")];
    private List<ColumnInfo> Columns => [.. columns.Where(c => c.Fixed != "left" && c.Fixed != "right")];
    private bool HasFixedLeft => columns.Any(c => c.Fixed == "left");
    private bool HasFixedRight => columns.Any(c => c.Fixed == "right");
    private bool Indeterminate => columns.Any(c => c.IsVisible) && columns.Count(c => c.IsVisible) < columns.Count;
    private bool CheckAll => columns.All(c => c.IsVisible);

    /// <summary>
    /// 取得或设置表格模型配置信息。
    /// </summary>
    [Parameter] public TableModel<TItem> Table { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        columns = Table.GetUserColumns();
    }

    private async Task CheckAllChangedAsync(bool value)
    {
        bool allChecked = CheckAll;
        columns.ForEach(c => c.IsVisible = !allChecked);
        await OnColumnChangedAsync();
    }

    private async Task OnReset(MouseEventArgs args)
    {
        if (!Context.UserTableSettings.ContainsKey(Table.SettingId))
            return;

        await SaveSettingAsync(null);
        columns = Table.GetUserColumns();
        await StateChangedAsync();
    }

    private async Task OnDropAsync(DragEventArgs e, ColumnInfo info)
    {
        if (info != null && dragging != null)
        {
            int index = columns.IndexOf(info);
            columns.Remove(dragging);
            columns.Insert(index, dragging);
            dragging = null;
            await StateChangedAsync();
            await OnColumnChangedAsync();
        }
    }

    private void OnDragStart(DragEventArgs e, ColumnInfo info)
    {
        e.DataTransfer.DropEffect = "move";
        e.DataTransfer.EffectAllowed = "move";
        dragging = info;
    }

    private async Task OnVisibleChange(ColumnInfo item, bool visible)
    {
        item.IsVisible = visible;
        await OnColumnChangedAsync();
    }

    private async Task OnWidthChange(ColumnInfo item, int? width)
    {
        item.Width = width;
        await OnColumnChangedAsync();
    }

    private async Task OnFixedChange(ColumnInfo item, int fix)
    {
        if (fix > 0)
            item.Fixed = fix == 1 ? "left" : "right";
        else
            item.Fixed = string.Empty;
        await OnColumnChangedAsync();
    }

    private async Task OnColumnChangedAsync()
    {
        var index = 0;
        var infos = new List<TableSettingInfo>();
        foreach (var item in columns)
        {
            infos.Add(new TableSettingInfo
            {
                Id = item.Id,
                Fixed = item.Fixed,
                IsVisible = item.IsVisible,
                Width = item.Width,
                Sort = ++index
            });
        }
        await SaveSettingAsync(infos);
    }

    private async Task SaveSettingAsync(List<TableSettingInfo> infos)
    {
        if (infos == null)
            Context.UserTableSettings.Remove(Table.SettingId);
        else
            Context.UserTableSettings[Table.SettingId] = infos;
        Table.Columns = Table.GetUserColumns();
        await Admin.SaveUserSettingAsync(new SettingFormInfo { BizType = Table.SettingId, BizData = infos });
        Table.Reload();
    }
}