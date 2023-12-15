using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class UploadField<TItem> : BaseComponent where TItem : class, new()
{
    private List<SysFile> sysFiles;

    [Parameter] public FieldModel<TItem> Model { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        sysFiles = await Platform.File.GetFilesAsync($"{Model.Value}");
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!Model.Form.IsView)
        {
            builder.Component<InputFile>()
                   .Add("multiple", Model.Column.IsMultiFile)
                   .Set(c => c.OnChange, this.Callback<InputFileChangeEventArgs>(OnFilesChanged))
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
                        if (!Model.Form.IsView)
                        {
                            builder.Span("kui-link danger", "删除", this.Callback(() => OnDeleteFile(item)));
                        }
                        builder.DownloadLink(item.Name, item.FileUrl);
                    });
                }
            });
        }
    }

    private void OnFilesChanged(InputFileChangeEventArgs e)
    {
        var column = Model.Column;
        if (column.IsMultiFile)
        {
            var files = e.GetMultipleFiles();
            Model.Form.Files[column.Property.Name] = [.. files];
        }
        else
        {
            Model.Form.Files[column.Property.Name] = [e.File];
        }
    }

    private void OnDeleteFile(SysFile item)
    {
        UI.Confirm($"确定要删除{item.Name}？", async () =>
        {
            await Platform.File.DeleteFileAsync(item);
            sysFiles.Remove(item);
            StateChanged();
        });
    }
}