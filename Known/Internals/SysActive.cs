namespace Known.Internals;

/// <summary>
/// 系统激活组件类。
/// </summary>
public class SysActive : BaseComponent
{
    private ISystemService Service;
    private FormModel<ActiveInfo> model;

    /// <summary>
    /// 取得或设置授权状态。
    /// </summary>
    [Parameter] public string AuthStatus { get; set; }

    /// <summary>
    /// 取得或设置激活信息。
    /// </summary>
    [Parameter] public ActiveInfo Data { get; set; }

    /// <summary>
    /// 取得或设置激活事件委托。
    /// </summary>
    [Parameter] public Action<Result> OnActive { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<ISystemService>();
        model = new FormModel<ActiveInfo>(this) { Data = Data };
        model.AddRow().AddColumn(c => c.ProductId, c => c.ReadOnly = true);
        model.AddRow().AddColumn(c => c.ProductKey, c => c.Required = true);
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-card", () =>
        {
            builder.Result("403", AuthStatus);
            builder.Div("kui-form-auth", () =>
            {
                builder.Form(model);
                builder.FormPageButton(() =>
                {
                    builder.Button(Language.OK, this.Callback<MouseEventArgs>(OnAuthAsync));
                });
            });
        });
    }

    private async Task OnAuthAsync(MouseEventArgs args)
    {
        if (!model.Validate())
            return;

        var result = await Service.SaveProductKeyAsync(model.Data);
        OnActive?.Invoke(result);
    }
}