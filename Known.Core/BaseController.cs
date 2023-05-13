namespace Known.Core;

public class BaseController : ControllerBase
{
    private readonly Context context = new();
    protected Context Context
    {
        get
        {
            var token = Request.Headers[Constants.KeyToken];
            context.CurrentUser = UserService.GetUserByToken(token);
            return context;
        }
    }

    protected static dynamic GetDynamicModel(object model)
    {
        var json = Utils.ToJson(model);
        return Utils.ToDynamic(json);
    }

    protected static UploadFormInfo GetUploadFormInfo(string model)
    {
        return new UploadFormInfo
        {
            Model = Utils.ToDynamic(model),
            Files = new Dictionary<string, IAttachFile>(),
            MultiFiles = new Dictionary<string, List<IAttachFile>>()
        };
    }

    protected static List<IAttachFile> GetAttachFiles(IEnumerable<IFormFile> files)
    {
        var attchFiles = new List<IAttachFile>();
        if (files != null && files.Any())
        {
            foreach (var item in files)
            {
                attchFiles.Add(new FormAttachFile(item));
            }
        }
        return attchFiles;
    }
}