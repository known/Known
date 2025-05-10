namespace Known.Internals;

class SysActive : BaseComponent
{
    private FormModel<SystemInfo> model;

    [Parameter] public string AuthStatus { get; set; }
    [Parameter] public SystemInfo Data { get; set; }
    [Parameter] public Func<SystemInfo, Task> OnCheck { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        model = new FormModel<SystemInfo>(this) { Data = Data };
        model.AddRow().AddColumn(c => c.ProductId, c => c.ReadOnly = true);
        model.AddRow().AddColumn(c => c.ProductKey, c => c.Required = true);
    }

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

        await OnCheck.Invoke(model.Data);
    }
}