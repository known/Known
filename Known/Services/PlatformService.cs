namespace Known.Services;

/// <summary>
/// 框架平台数据服务接口。
/// </summary>
public partial interface IPlatformService : IService
{
}

[Service]
partial class PlatformService(Context context) : ServiceBase(context), IPlatformService
{
}

[Client]
partial class PlatformClient(HttpClient http) : ClientBase(http), IPlatformService
{
}