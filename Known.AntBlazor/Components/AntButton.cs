namespace Known.AntBlazor.Components;

public class AntButton : BaseComponent
{
    private bool isLoad;

    [Parameter] public bool Block { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Type { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        var isDanger = Type == "danger";
        if (isDanger)
            Type = ButtonType.Primary;
        builder.Component<Button>()
               .Set(c => c.Block, Block)
               .Set(c => c.Icon, Icon)
               .Set(c => c.Type, Type)
               .Set(c => c.Class, Class)
               .Set(c => c.Danger, isDanger)
               .Set(c => c.Disabled, !Enabled)
               .Set(c => c.Loading, isLoad)
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(OnButtonClick))
               .Set(c => c.ChildContent, b => b.Text(Name))
               .Build();
    }

    private async void OnButtonClick(MouseEventArgs args)
    {
        if (isLoad || !OnClick.HasDelegate)
            return;

        isLoad = true;
        await OnClick.InvokeAsync(args);
        isLoad = false;
    }
}