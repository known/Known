﻿namespace Known.Internals;

class TableSetting<TItem> : BaseComponent where TItem : class, new()
{
    private List<ColumnInfo> columns = [];
    private ColumnInfo dragging;

    [Parameter] public TableModel<TItem> Table { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        columns = Table.GetUserColumns();
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("setting", () =>
        {
            builder.Dropdown(new DropdownModel
            {
                Tooltip = Language["ColumnSettings"],
                TriggerType = "Click",
                Icon = "setting",
                Overlay = BuildOverlay
            });
        });
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        builder.Overlay("kui-table-setting", () =>
        {
            builder.Div("title", () =>
            {
                builder.CheckBox(new InputModel<bool>
                {
                    Label = Language["ColumnSettings"],
                    Indeterminate = Indeterminate,
                    Value = CheckAll,
                    ValueChanged = this.Callback<bool>(CheckAllChangedAsync)
                });
                builder.Button(Language.Reset, this.Callback<MouseEventArgs>(OnReset));
            });
            foreach (var item in columns)
            {
                builder.Div().Class("item").Draggable()
                       .OnDrop(this.Callback<DragEventArgs>(e => OnDropAsync(e, item)))
                       .OnDragStart(this.Callback<DragEventArgs>(e => OnDragStart(e, item)))
                       .Child(() => BuildSettingItem(builder, item));
            }
        });
    }

    private void BuildSettingItem(RenderTreeBuilder builder, ColumnInfo item)
    {
        builder.Icon("pause");
        builder.CheckBox(new InputModel<bool>
        {
            Label = Language.GetFieldName<TItem>(item),
            Value = item.IsVisible,
            ValueChanged = this.Callback<bool>(async v =>
            {
                item.IsVisible = v;
                await OnColumnChangedAsync();
            })
        });
        builder.Number(new InputModel<int?>
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
        await Table.StateChangeAsync();
    }
}