using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class StepForm : BaseComponent
{
    [Parameter] public StepModel Model { get; set; }
    [Parameter] public bool IsView { get; set; }
    [Parameter] public Func<bool, Task<bool>> OnSave { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        UI.BuildSteps(builder, Model);
        builder.Div("steps-content", () =>
        {
            builder.Fragment(Model.Items[Model.Current].Content);
        });
        builder.Div("form-action", () =>
        {
            if (Model.Current > 0)
                UI.Button(builder, "上一步", this.Callback<MouseEventArgs>(e => OnPrevClick()), "primary");
            if (Model.Current < Model.Items.Count - 1)
                UI.Button(builder, "下一步", this.Callback<MouseEventArgs>(e => OnNextClick()), "primary");
            if (Model.Current == Model.Items.Count - 1 && !IsView)
                UI.Button(builder, "完成", this.Callback<MouseEventArgs>(e => OnComplete()), "primary");
        });
    }

    private async void OnPrevClick()
    {
        if (!await SaveAsync())
            return;

        Model.Current--;
    }

    private async void OnNextClick()
    {
        if (!await SaveAsync())
            return;

        Model.Current++;
    }

    private async void OnComplete()
    {
        await SaveAsync(true);
    }

    private async Task<bool> SaveAsync(bool isClose = false)
    {
        if (IsView)
            return true;

        return await OnSave?.Invoke(isClose);
    }
}