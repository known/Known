# 上传文件

本章介绍学习如何上传文件，上传文件的场景有如下3种：

- 通过点击按钮导入数据
- 通过表单Upload字段选择附件与表单数据同时保存
- 富文本编辑器文件上传

## 1. 导入文件

- 在页面添加按钮样式的Upload组件

**1）TestClient**

```csharp
public class TestClient : ClientBase
{
    public TestClient(Context context) : base(context) { }

    public Task<Result> ImportFileAsync(HttpContent content) => Context.PostAsync("Test/ImportFile", content);
}
```

**2）前端示例**

```csharp
//构建导入按钮
protected override void BuildRenderTree(RenderTreeBuilder builder)
{
    builder.Field<Upload>("Bill").Visible(!ReadOnly)
           .Set(f => f.IsButton, true)
           .Set(f => f.ButtonText, "导入")
           .Set(f => f.OnFilesChanged, OnFilesChanged)
           .Build();
}
//选择文件事件
private async void OnFilesChanged(List<IBrowserFile> files)
{
    var file = files.FirstOrDefault();
    if (file == null)
    {
        UI.Alert("请选择附件！");
        return;
    }

    UI.ShowLoading("导入中...");
    //封装HttpContent数据
    using var content = new MultipartFormDataContent();
    //导入文件附件的其他数据
    var json = Utils.ToJson(Data);
    var modelContent = new StringContent(json);
    content.Add(modelContent, "\"model\"");
    //添加文件流
    var fileContent = new StreamContent(file.OpenReadStream(Upload.MaxLength));
    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
    content.Add(fileContent, "\"fileTest\"", file.Name);
    //请求后端API导入文件
    var result = await Client.Test.ImportFileAsync(content);
    if (!result.IsValid)
    {
        UI.Alert(result.Message);
        UI.CloseLoading();
        return;
    }

    UI.CloseLoading();
    UI.Toast("导入成功！");
}
```

> 注意：model和fileTest应与Controller接受参数名称一致

**3）后端示例**

```csharp
//Controller
[Route("[controller]")]
public class TestController : BaseController
{
    private TestService Service => new(Context);

    [HttpPost("[action]")]
    public Result ImportFile([FromForm] string model, [FromForm] IEnumerable<IFormFile> fileTest)
    {
        var info = new UploadFormInfo(model);
        info.Files["Test"] = GetAttachFiles(fileTest);
        return Service.ImportFile(info);
    }
}

//Service
class TestService : ServiceBase
{
    internal TestService(Context context) : base(context) { }

    internal Result ImportFile(UploadFormInfo info)
    {
        //读取model动态对象数据
        var id = (string)info.Model.Id;
        
        //读取文件流，本例假设文件是Excel
        var stream = info.Files["Test"][0].GetStream();
        //创建Excel对象
        var excel = ExcelFactory.Create(stream);
        if (excel == null)
            return Result.Error("Excel创建失败！");

        //读取Excel数据
        var lines = excel.SheetToDictionaries(0);
        for (int i = 0; i < lines.Count; i++)
        {
            //读取行列数据
            var item = lines[i]["Excel第一行标题"];
        }

        return Database.Transaction("导入", db =>
        {
            //将数据保存只数据库
        });
    }
}
```

