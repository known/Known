namespace Known.Services;

/// <summary>
/// 管理后台数据服务接口。
/// </summary>
public partial interface IAdminService : IService
{
}

[Client]
partial class AdminClient(HttpClient http) : ClientBase(http), IAdminService
{
}

[WebApi, Service]
partial class AdminService(Context context) : ServiceBase(context), IAdminService
{
}