namespace Known.Internals;

class AutoFormPage : BaseForm, IAutoPage
{
    [Parameter] public string PageId { get; set; }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected override void BuildForm(RenderTreeBuilder builder)
    {
        var model = new FormModel<Dictionary<string, object>>(this);
        builder.Form(model);
    }
}