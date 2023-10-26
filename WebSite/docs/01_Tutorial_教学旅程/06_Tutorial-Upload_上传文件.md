# 上传文件

本章介绍学习如何上传文件，上传文件的场景有如下2种：

- 通过点击按钮导入数据
- 通过表单Upload字段选择附件与表单数据同时保存

## 1. 点击按钮导入文件

**1）Client类**

```csharp
public class TestClient : ClientBase
{
    public TestClient(Context context) : base(context) { }
    //导入文件
    public Task<Result> ImportFileAsync(HttpContent content) => Context.PostAsync("Test/ImportFile", content);
}
```

**2）前端**

- 在页面添加按钮样式的Upload组件
- 在选择文件事件中封装HttpContent

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

> 注意：model和fileTest应与Controller接收参数名称一致

**3）后端**

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
        //构造附件类实例
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

## 2. 带附件字段表单

- 使用SubmitFilesAsync保存带附件的表单数据

**1）Client类**

```csharp
public class TestClient : ClientBase
{
    public TestClient(Context context) : base(context) { }
    //保存数据
    public Task<Result> SaveDataAsync(HttpContent content) => Context.PostAsync("Test/SaveData", content);
}
```

**2）前端**

```csharp
class TestForm : WebForm<TbTest>
{
    //建造表单字段
    protected override void BuildFields(FieldBuilder<TbTest> builder)
    {
        builder.Hidden(f => f.Id);
        builder.Table(table =>
        {
            //此处省略其他字段
            table.Tr(attr =>
            {
                //Upload字段
                table.Field<Upload>(f => f.BizFile).ColSpan(3)
                     .Set(f => f.IsMultiple, true) //是否上传多个文件
                     .Set(f => f.CanDelete, true)  //编辑模式是否可删除
                     .Build();
            });
        });
    }
    //建造按钮
    protected override void BuildButtons(RenderTreeBuilder builder)
    {
        builder.Button(FormButton.Save, Callback(OnSave), !ReadOnly);
        base.BuildButtons(builder);
    }
    //保存按钮方法
    private void OnSave() => SubmitFilesAsync(Client.Test.SaveDataAsync);
}
```

**3）后端**

```csharp
//Controller
[Route("[controller]")]
public class TestController : BaseController
{
    private TestService Service => new(Context);
    //附件参数名称规范：file[字段属性名]，该实体附件字段名为：BizFile
    [HttpPost("[action]")]
    public Result SaveData([FromForm] string model, [FromForm] IEnumerable<IFormFile> fileBizFile)
    {
        var info = new UploadFormInfo(model);
        //构造附件类实例
        info.Files[nameof(TbTest.BizFile)] = GetAttachFiles(fileBizFile);
        return Service.SaveData(info);
    }
}

//Service
class TestService : ServiceBase
{
    internal TestService(Context context) : base(context) { }

    //保存数据
    internal Result SaveData(UploadFormInfo info)
    {
        var entity = Database.QueryById<TbTest>((string)info.Model.Id);
        entity ??= new TbTest();
        entity.FillModel(info.Model);//填充表单数据到实体对象
        var vr = entity.Validate();  //验证实体对象
        if (!vr.IsValid)
            return vr;

        var user = CurrentUser;
        var bizType = "TestFiles";//业务类型，将附件保存在该名称的文件夹中
        //保存真实附件至磁盘
        var bizFiles = GetAttachFiles(info, user, nameof(TbTest.BizFile), bizType);
        return Database.Transaction(Language.Save, db =>
        {
            //保存附件，所有附件记录均保存在SysFile表中
            //AddFiles方法为添加，SaveFile可替换存在的附件
            //支持保存图片的缩略图
            PlatformHelper.AddFiles(db, bizFiles, entity.Id, bizType);
            //附件字段存储实体ID和业务类型
            //表单页面根据该字段自动显示附件列表，可删除和下载
            entity.BizFile = $"{entity.Id}_{bizType}";
            db.Save(entity);
        }, entity);
    }
}
```
