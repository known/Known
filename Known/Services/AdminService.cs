﻿namespace Known.Services;

/// <summary>
/// 管理后台数据服务接口。
/// </summary>
public partial interface IAdminService : IService
{
}

[Service]
partial class AdminService(Context context) : ServiceBase(context), IAdminService
{
    private static readonly Dictionary<string, string> Configs = [];
}

[Client]
partial class AdminClient(HttpClient http) : ClientBase(http), IAdminService
{
}