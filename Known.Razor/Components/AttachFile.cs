namespace Known.Razor.Components;

public class AttachFile : BaseComponent
{
    private List<SysFile> files;

    [Parameter] public bool CanUpload { get; set; }
    [Parameter] public string Icon { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public List<SysFile> Files { get; set; }
    [Parameter] public FileFormInfo Model { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildButton(builder);
        BuildFiles(builder);
    }

    private void BuildButton(RenderTreeBuilder builder)
    {
        if (!CanUpload)
            return;

        builder.Div("attachButton", attr =>
        {
            builder.IconName(Icon, Text ?? "上传");
            builder.Component<InputFile>(attr =>
            {
                //.Add("accept", Constants.MimeImage)
                attr.Add("class", "upload")
                    .Add("multiple", true)
                    .Set(c => c.OnChange, Callback<InputFileChangeEventArgs>(async e => await OnFilesChanged(e)));
            });
        });
    }

    private void BuildFiles(RenderTreeBuilder builder)
    {
        files ??= Files?.Where(f => f.BizId == Model?.BizId).ToList();
        if (files == null || files.Count == 0)
            return;

        builder.Div("attachFiles", attr =>
        {
            foreach (var item in files)
            {
                builder.Div("attachFile", attr =>
                {
                    builder.Link("删除", Callback(e => OnDeleteFile(item)), "danger");
                    var url = item.FileUrl;
                    builder.Anchor(item.Name, url.OriginalUrl, url.FileName);
                });
            }
        });
    }

    private async Task OnFilesChanged(InputFileChangeEventArgs e)
    {
        if (e.FileCount > 10)
        {
            UI.Toast("一次最多上传10个文件！");
            return;
        }

        var upload = false;
        using var content = new MultipartFormDataContent();
        var json = Utils.ToJson(Model);
        var modelContent = new StringContent(json);
        content.Add(modelContent, "\"model\"");

        foreach (var item in e.GetMultipleFiles())
        {
            var fileContent = new StreamContent(item.OpenReadStream(Upload.MaxLength));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.ContentType);
            content.Add(fileContent, "\"fileUpload\"", item.Name);
            upload = true;
        }

        if (upload)
        {
            var result = await Platform.File.UploadFilesAsync(content);
            UI.Result(result, () =>
            {
                files ??= new List<SysFile>();
                var data = result.DataAs<List<SysFile>>();
                if (data != null)
                {
                    files = files.Concat(data).ToList();
                    StateChanged();
                }
            });
        }
    }

    private void OnDeleteFile(SysFile item)
    {
        UI.Confirm($"确定要删除{item.Name}？", async () =>
        {
            var result = await Platform.File.DeleteFileAsync(item);
            UI.Result(result, () =>
            {
                files?.Remove(item);
                StateChanged();
            });
        });
    }
}