﻿namespace Known.Blazor;

public class StepForm : BaseComponent
{
    [Parameter] public StepModel Model { get; set; }
    [Parameter] public bool IsView { get; set; }
    [Parameter] public int? StepCount { get; set; }
    [Parameter] public Func<bool, Task<bool>> OnSave { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        StepCount ??= Model.Items.Count;
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div($"kui-steps-form {Model.Direction}", () =>
        {
            UI.BuildSteps(builder, Model);
            builder.Div(() =>
            {
                builder.Div("kui-steps-content", () =>
                {
                    builder.Fragment(Model.Items[Model.Current].Content);
                });
                builder.FormAction(() =>
                {
                    if (Model.Current > 0)
                        UI.Button(builder, Language["Button.Previous"], this.Callback<MouseEventArgs>(OnPrevClickAsync), "primary");
                    if (Model.Current < StepCount - 1)
                        UI.Button(builder, Language["Button.Next"], this.Callback<MouseEventArgs>(OnNextClickAsync), "primary");
                    if (Model.Current == StepCount - 1 && !IsView)
                        UI.Button(builder, Language["Button.Finish"], this.Callback<MouseEventArgs>(OnCompleteAsync), "primary");
                });
            });
        });
    }

    public void SetStepCount(int count)
    {
        StepCount = count;
        StateChanged();
    }

    private async void OnPrevClickAsync(MouseEventArgs arg)
    {
        if (!await SaveAsync())
            return;

        Model.Current--;
    }

    private async void OnNextClickAsync(MouseEventArgs arg)
    {
        if (!await SaveAsync())
            return;

        Model.Current++;
    }

    private async void OnCompleteAsync(MouseEventArgs arg)
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