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
        builder.Div("sys-active", attr =>
        {
            builder.Component<Empty>()
                   .Set(c => c.Icon, "fa fa-shield")
                   .Set(c => c.Text, KRConfig.AuthStatus)
                   .Build();

            builder.Field<Text>("产品ID：", "").Value(info?.ProductId).ReadOnly(true).Build();
            builder.Field<Text>("产品密钥：", "ProductKey").Value(info?.ProductKey).Build(value => productKey = value);
            builder.Div("form-button", attr => builder.Button("授权", Callback(OnAuth), StyleType.Primary));
        });
    }

    private async void OnAuth()
    {
        productKey.ShowError(false);
        var key = productKey.Value;
        if (string.IsNullOrWhiteSpace(key))
        {
            productKey.ShowError(true);
            return;
        }

        info.ProductKey = key;
        var result = await Platform.System.SaveKeyAsync(info);
        UI.Result(result);
        OnCheck?.Invoke(result.IsValid);
    }
}