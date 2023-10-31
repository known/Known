namespace Known.Razor;

public class KAttach : BaseComponent
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
            builder.Component<InputFile>()
                 .Add("class", "upload")
                 .Add("multiple", true)
                 .Set(c => c.OnChange, Callback<InputFileChangeEventArgs>(async e => await OnFilesChangedAsync(e)))
                 .Build();
            //.Add("accept", Constants.MimeImage)
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

    private async Task OnFilesChangedAsync(InputFileChangeEventArgs e)
    {
        if (e.FileCount > 10)
        {
            UI.Toast("一次最多上传10个文件！");
            return;
        }

        var attachs = new List<IAttachFile>();
        foreach (var item in e.GetMultipleFiles())
        {
            attachs.Add(new BlazorAttachFile(item));
        }

        var json = Utils.ToJson(Model);
        var info = new UploadFormInfo
        {
            Model = Utils.ToDynamic(json),
            Files = new Dictionary<string, List<IAttachFile>>
            {
                ["Upload"] = attachs
            }
        };

        var result = await Platform.File.UploadFilesAsync(info);
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