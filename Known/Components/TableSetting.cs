namespace Known.Components;

class TableSetting<TItem> : BaseComponent where TItem : class, new()
{
    private ISettingService Service;
    private List<ColumnInfo> columns = [];
    private ColumnInfo dragging;

    [Parameter] public TableModel<TItem> Table { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISettingService>();
        columns = Context.GetUserTableColumns(Table);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("setting", () =>
        {
            UI.BuildDropdown(builder, new DropdownModel
            {
                Tooltip = Language["Designer.ColumnSettings"],
                TriggerType = "Click",
                Icon = "setting",
                Overlay = BuildOverlay
            });
        });
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        builder.Div("kui-table-setting", () =>
        {
            builder.Div("title", () =>
            {
                UI.BuildCheckBox(builder, new InputModel<bool>
                {
                    Label = Language["Designer.ColumnSettings"],
                    Indeterminate = Indeterminate,
                    Value = CheckAll,
                    ValueChanged = this.Callback<bool>(CheckAllChangedAsync)
                });
            });
            foreach (var item in columns)
            {
                builder.Div().Class("item").Draggable()
                       .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, item)))
                       .OnDragStart(this.Callback<DragEventArgs>(e => OnDragStart(e, item)))
                       .Children(() => BuildSettingItem(builder, item))
                       .Close();
            }
        });
    }

    private void BuildSettingItem(RenderTreeBuilder builder, ColumnInfo item)
    {
        builder.Icon("pause");
        UI.BuildCheckBox(builder, new InputModel<bool>
        {
            Label = item.Name,
            Value = item.IsVisible,
            ValueChanged = this.Callback<bool>(async v =>
            {
                item.IsVisible = v;
                await OnColumnChangedAsync();
            })
        });
        UI.BuildNumber(builder, new InputModel<int?>
        {
            Value = item.Width,
            ValueChanged = this.Callback<int?>(async v =>
            {
                item.Width = v;
                await OnColumnChangedAsync();
            })
        });
    }

    private bool Indeterminate => columns.Any(c => c.IsVisible) && columns.Count(c => c.IsVisible) < columns.Count;
    private bool CheckAll => columns.All(c => c.IsVisible);

    private async Task CheckAllChangedAsync(bool value)
    {
        bool allChecked = CheckAll;
        columns.ForEach(c => c.IsVisible = !allChecked);
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
                IsVisible = item.IsVisible,
                Width = item.Width ?? 100,
                Sort = ++index
            });
        }
        await Service.SaveUserSettingFormAsync(new SettingFormInfo
        {
            BizType = Table.SettingId,
            BizData = infos
        });
        Context.UserTableSettings[Table.SettingId] = infos;
        await Table.StateChangeAsync();
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
}