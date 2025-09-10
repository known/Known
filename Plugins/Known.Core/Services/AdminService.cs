namespace Known.Services;

[WebApi, Service]
partial class AdminService(Context context) : ServiceBase(context), IAdminService
{
}