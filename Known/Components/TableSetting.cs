
namespace Known.Components;

class TableSetting<TItem> : BaseComponent where TItem : class, new()
{
    private string SettingKey => $"UserTable_{Context.Current.Id}";
    private ISettingService Service;
    private List<ColumnInfo> columns = [];
    private ColumnInfo dragging;

    [Parameter] public TableModel<TItem> Table { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISettingService>();
        columns = Table.Columns.Where(c => c.IsVisible).ToList();
        Context.UserTableSettings.TryGetValue(SettingKey, out List<TableSettingInfo> settings);
        if (settings != null && settings.Count > 0)
        {
            foreach (var item in columns)
            {
                var setting = settings.FirstOrDefault(c => c.Id == item.Id);
                if (setting != null)
                {
                    item.IsVisible = setting.IsVisible;
                    item.Width = setting.Width;
                    item.Sort = setting.Sort;
                }
            }
            columns = [.. columns.OrderBy(c => c.Sort)];
        }
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
                    ValueChanged = this.Callback<bool>(v => CheckAllChanged())
                });
            });
            foreach (var item in columns)
            {
                builder.Div().Class("item")
                       .Attribute("draggable", "true")
                       .Attribute("ondrop", this.Callback<DragEventArgs>(e => OnDrop(e, item)))
                       .Attribute("ondragstart", this.Callback<DragEventArgs>(e => OnDragStart(e, item)))
                       .Attribute("ondragover", "event.preventDefault()")
                       .Children(() =>
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
                       })
                       .Close();
            }
        });
    }

    private bool Indeterminate => columns.Any(c => c.IsVisible) && columns.Count(c => c.IsVisible) < columns.Count;
    private bool CheckAll => columns.All(c => c.IsVisible);

    private async void CheckAllChanged()
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
            BizType = SettingKey,
            BizData = infos
        });
        Context.UserTableSettings[SettingKey] = infos;
        await Table.ChangeAsync();
    }

    private async void OnDrop(DragEventArgs e, ColumnInfo info)
    {
        if (info != null && dragging != null)
        {
            int index = columns.IndexOf(info);
            columns.Remove(dragging);
            columns.Insert(index, dragging);
            dragging = null;
            StateChanged();
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