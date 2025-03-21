namespace Known.Internals;

class UploadField<TItem> : KUpload where TItem : class, new()
{
    [Parameter] public FieldModel<TItem> Model { get; set; }

    protected override async Task OnInitAsync()
    {
        Id = Model.Column.Id;
        ReadOnly = Model.Form.IsView;
        Value = Model.Value?.ToString();
        MultiFile = Model.Column.MultiFile;
        TemplateUrl = Model.Column.TemplateUrl;
        OnFilesChanged = files =>
        {
            Model.Form.Files[Id] = files;
            Model.Value = Id;
            return Task.CompletedTask;
        };
        await base.OnInitAsync();
    }
}