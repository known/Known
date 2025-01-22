namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public partial interface IPlatformService : IService
{
}

[WebApi]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
}

partial class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
}