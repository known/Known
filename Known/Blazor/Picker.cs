using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class Picker : BaseComponent
{
    [Parameter] public string Value { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public RenderFragment Content { get; set; }
    [Parameter] public Action<object> OnPicked { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildText(builder, new InputModel<string> { Value = Value, Disabled = true });
        builder.Span().Class("kui-pick fa fa-ellipsis-h").OnClick(this.Callback(ShowModal)).Close();
    }

    private void ShowModal()
    {
        if (ReadOnly)
            return;

        var model = new DialogModel { Title = Title, Content = Content };
        model.OnOk = async () =>
        {
            OnPicked?.Invoke("test");
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }
}