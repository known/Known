using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

public class KUpload : BaseComponent
{
    private List<SysFile> sysFiles;

    [Parameter] public string Value { get; set; }
    [Parameter] public bool MultiFile { get; set; }
    [Parameter] public Action<List<IBrowserFile>> OnFilesChanged { get; set; }

    public async void SetValue(string value)
    {
        Value = value;
        sysFiles = await Platform.File.GetFilesAsync(Value);
        StateChanged();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        sysFiles = await Platform.File.GetFilesAsync(Value);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.Component<InputFile>()
                   .Add("multiple", MultiFile)
                   .Set(c => c.OnChange, this.Callback<InputFileChangeEventArgs>(OnInputFileChanged))
                   .Build();
        }

        if (sysFiles != null && sysFiles.Count > 0)
        {
            builder.Div("kui-form-files", () =>
            {
                foreach (var item in sysFiles)
                {
                    var url = item.FileUrl;
                    builder.Div(() =>
                    {
                        if (!ReadOnly)
                        {
                            builder.Span("kui-link danger", Language.Delete, this.Callback<MouseEventArgs>(e => OnDeleteFile(item)));
                        }
                        builder.DownloadLink(item.Name, item.FileUrl);
                    });
                }
            });
        }
    }

    private void OnInputFileChanged(InputFileChangeEventArgs e)
    {
        if (MultiFile)
        {
            var files = e.GetMultipleFiles();
            OnFilesChanged?.Invoke([.. files]);
        }
        else
        {
            OnFilesChanged?.Invoke([e.File]);
        }
    }

    private void OnDeleteFile(SysFile item)
    {
        var message = Language["Tip.ConfirmDelete"].Replace("{name}", item.Name);
        UI.Confirm(message, async () =>
        {
            await Platform.File.DeleteFileAsync(item);
            sysFiles.Remove(item);
            StateChanged();
        });
    }
}

class KUploadField<TItem> : KUpload where TItem : class, new()
{
    [Parameter] public FieldModel<TItem> Model { get; set; }

    protected override async Task OnInitAsync()
    {
        Id = Model.Column.Id;
        ReadOnly = Model.Form.IsView;
        Value = Model.Value?.ToString();
        MultiFile = Model.Column.MultiFile;
        OnFilesChanged = files => Model.Form.Files[Id] = files;
        await base.OnInitAsync();
    }
}