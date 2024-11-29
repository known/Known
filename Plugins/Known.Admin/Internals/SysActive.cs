﻿namespace Known.Internals;

class SysActive : BaseComponent
{
    private FormModel<SystemInfo> model;

    [Parameter] public Action<bool> OnCheck { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        model = new FormModel<SystemInfo>(this);
        model.Data = new SystemInfo();
        model.AddRow().AddColumn(c => c.ProductId, c => c.ReadOnly = true);
        model.AddRow().AddColumn(c => c.ProductKey, c => c.Required = true);
    }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    await base.OnAfterRenderAsync(firstRender);
    //    if (firstRender)
    //    {
    //        var info = await Data.GetInstallAsync();
    //        model.Data.ProductId = info.ProductId;
    //        model.Data.ProductKey = info.ProductKey;
    //        await StateChangedAsync();
    //    }
    //}

    //protected override void BuildRender(RenderTreeBuilder builder)
    //{
    //    UI.BuildResult(builder, "403", Config.AuthStatus);
    //    builder.Div("kui-form-auth", () =>
    //    {
    //        UI.BuildForm(builder, model);
    //        builder.FormPageButton(() =>
    //        {
    //            builder.Button(new ActionInfo(Context, "OK"), this.Callback<MouseEventArgs>(OnAuthAsync));
    //        });
    //    });
    //}

    //private async Task OnAuthAsync(MouseEventArgs args)
    //{
    //    if (!model.Validate())
    //        return;

    //    var result = await Data.SaveKeyAsync(model.Data);
    //    UI.Result(result, () =>
    //    {
    //        OnCheck?.Invoke(result.IsValid);
    //        return Task.CompletedTask;
    //    });
    //}
}