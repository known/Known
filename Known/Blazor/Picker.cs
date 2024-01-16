using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public abstract class BasePicker<TItem> : BaseComponent where TItem : class, new()
{
    public List<TItem> SelectedItems { get; } = [];
}

public class Picker<TComponent, TItem> : BaseComponent
    where TComponent : BasePicker<TItem>
    where TItem : class, new()
{
    private BasePicker<TItem> picker;

    [Parameter] public string Value { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public Action<List<TItem>> OnPicked { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildText(builder, new InputModel<string> { Value = Value, Disabled = true });
        builder.Span().Class("kui-pick fa fa-ellipsis-h").OnClick(this.Callback(ShowModal)).Close();
    }

    private void ShowModal()
    {
        if (ReadOnly)
            return;

        var model = new DialogModel { Title = Title, Content = BuildContent };
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