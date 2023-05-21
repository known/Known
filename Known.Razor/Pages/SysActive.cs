namespace Known.Razor.Pages;

class SysActive : BaseComponent
{
    private Text productKey;
    private SystemInfo info;

    [Parameter] public Action<bool> OnCheck { get; set; }

    protected override async Task OnInitializedAsync()
    {
        info = await Platform.System.GetSystemAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Empty>()
               .Set(c => c.Icon, "fa fa-shield")
               .Set(c => c.Text, KRConfig.AuthStatus)
               .Build();
        builder.Div("txt-center", $"产品ID：{info?.ProductId}");
        builder.Div("txt-center inline", attr =>
        {
            builder.Component<Text>()
                   .Add(nameof(Text.Id), "ProductKey")
                   .Add(nameof(Text.Value), info?.ProductKey)
                   .Build(value => productKey = value);
            builder.Button("授权", Callback(OnAuth));
        });
    }

    private async void OnAuth()
    {
        var key = productKey.Value;
        if (string.IsNullOrWhiteSpace(key))
            return;

        info.ProductKey = key;
        var result = await Platform.System.SaveSystemAsync(info);
        OnCheck?.Invoke(result.IsValid);
    }
}