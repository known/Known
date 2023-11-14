namespace Known.Razor;

public class SysSystem : BasePage
{
    protected SystemInfo Model { get; private set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Model = await Platform.System.GetSystemAsync();
    }

    protected Task<Result> SaveModelAsync() => Platform.System.SaveSystemAsync(Model);
}