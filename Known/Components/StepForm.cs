namespace Known.Components;

public class StepForm : BaseComponent
{
    [Parameter] public StepModel Model { get; set; }
    [Parameter] public bool IsView { get; set; }
    [Parameter] public bool IsStepSave { get; set; }
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
                    if (Model.Items != null && Model.Items.Count > 0)
                        builder.Fragment(Model.Items[Model.Current].Content);
                });
                builder.FormAction(() =>
                {
                    if (Model.Current > 0)
                        builder.Button(Language["Button.Previous"], this.Callback<MouseEventArgs>(OnPrevClickAsync));
                    if (Model.Current < StepCount - 1)
                        builder.Button(Language["Button.Next"], this.Callback<MouseEventArgs>(OnNextClickAsync));
                    if (Model.Current == StepCount - 1 && !IsView)
                        builder.Button(Language["Button.Finish"], this.Callback<MouseEventArgs>(OnCompleteAsync));
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
        if (IsStepSave)
        {
            if (!await SaveAsync())
                return;
        }

        Model.Current--;
    }

    private async void OnNextClickAsync(MouseEventArgs arg)
    {
        if (IsStepSave)
        {
            if (!await SaveAsync())
                return;
        }

        Model.Current++;
    }

    private async void OnCompleteAsync(MouseEventArgs arg)
    {
        await SaveAsync(true);
    }

    private async Task<bool> SaveAsync(bool isComplete = false)
    {
        if (IsView)
            return true;

        return await OnSave?.Invoke(isComplete);
    }
}