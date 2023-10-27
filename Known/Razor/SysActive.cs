namespace Known.Razor;

class SysActive : BaseComponent
{
    private KText productKey;
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
            builder.Component<KEmpty>()
                   .Set(c => c.Icon, "fa fa-shield")
                   .Set(c => c.Text, Config.AuthStatus)
                   .Build();

            builder.Field<KText>("产品ID：", "").Value(info?.ProductId).ReadOnly(true).Build();
            builder.Field<KText>("产品密钥：", "ProductKey").Value(info?.ProductKey).Build(value => productKey = value);
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