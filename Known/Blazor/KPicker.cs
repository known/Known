﻿namespace Known.Blazor;

public abstract class BasePicker<TItem> : BaseComponent where TItem : class, new()
{
    protected TableModel<TItem> Table;

    public bool IsMulti { get; set; }
    public virtual List<TItem> SelectedItems => Table?.SelectedRows?.ToList();

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table = new TableModel<TItem>(Context)
        {
            SelectType = IsMulti ? TableSelectType.Checkbox : TableSelectType.Radio,
            ShowPager = true
        };
    }

    protected override void BuildRender(RenderTreeBuilder builder) => builder.BuildTable(Table);
}

public class KPicker<TComponent, TItem> : BaseComponent
    where TComponent : BasePicker<TItem>
    where TItem : class, new()
{
    private BasePicker<TItem> picker;

    [Parameter] public double? Width { get; set; }
    [Parameter] public bool AllowClear { get; set; }
    [Parameter] public string Value { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public Action<List<TItem>> OnPicked { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        UI.BuildText(builder, new InputModel<string> { Value = Value, Disabled = true });

        if (!ReadOnly)
        {
            if (AllowClear)
                builder.Icon("kui-pick-clear fa fa-close", this.Callback<MouseEventArgs>(OnClear));
            builder.Icon("kui-pick fa fa-ellipsis-h", this.Callback<MouseEventArgs>(ShowModal));
        }
    }

    private void OnClear(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        Value = string.Empty;
        OnPicked?.Invoke(null);
    }

    private void ShowModal(MouseEventArgs args)
    {
        if (ReadOnly)
            return;

        var model = new DialogModel { Title = Title, Width = Width, Content = BuildContent };
        model.OnOk = async () =>
        {
            var items = picker?.SelectedItems;
            if (items == null || items.Count == 0)
            {
                UI.Error(Language.SelectOneAtLeast);
                return;
            }

            OnPicked?.Invoke(items);
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Component<TComponent>().Build(value => picker = value);
    }
}