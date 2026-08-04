using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Known.Filters;

class AuthActionFilter : IAsyncActionFilter
{
    // 零模块用户可访问的自助/引导接口（登录后渲染首页、修改密码、编辑个人中心、退出登录等）
    // 控制器与动作名均去掉 Service/Async 后缀，与 ApiConvention 生成的路由保持一致
    private static readonly HashSet<string> SelfServiceApis =
    [
        "Admin/GetAdmin",
        "Admin/GetUserModuleIds",
        "Admin/UpdateUser",
        "Admin/UpdatePassword",
        "Admin/UpdateAvatar",
        "Admin/SignOut",
        "Admin/SaveUserSetting",
        "System/GetSystem"
    ];

    // 用户模块缓存键与有效期，避免每次请求都查数据库
    private const string ModuleIdsCacheKey = "ModuleIds:{0}";
    private static readonly TimeSpan ModuleIdsCacheTime = TimeSpan.FromMinutes(5);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var action = context.ActionDescriptor as ControllerActionDescriptor;
        var attribute = action.ControllerTypeInfo.GetCustomAttribute<AnonymousAttribute>(false);
        if (attribute != null)
        {
            await next();
            return;
        }

        attribute = action.MethodInfo.GetCustomAttribute<AnonymousAttribute>(false);
        if (attribute != null)
        {
            await next();
            return;
        }

        var request = context.HttpContext.Request;
        if (!request.Headers.TryGetValue(Constants.KeyToken, out var token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var user = Cache.GetUserByToken(token);
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // 自助/引导接口对任意已登录用户放行，无需查询模块权限
        if (!SelfServiceApis.Contains(GetApiKey(action)))
        {
            // 管理员或未启用角色权限时不受模块限制
            if (!user.IsAdmin() && !CoreConfig.IsNoRole)
            {
                var moduleIds = await GetUserModuleIdsAsync(user);
                // 零模块用户：未分配任何角色模块，拒绝调用所有非匿名接口
                if (moduleIds == null || moduleIds.Count == 0)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }

        if (context.Controller is IService service)
            service.Context.CurrentUser = user;

        await next();
    }

    private static string GetApiKey(ControllerActionDescriptor action)
    {
        var controller = action.ControllerName;
        if (controller.EndsWith("Service", StringComparison.Ordinal))
            controller = controller[..^"Service".Length];

        var method = action.ActionName;
        if (method.EndsWith("Async", StringComparison.Ordinal))
            method = method[..^"Async".Length];

        return $"{controller}/{method}";
    }

    private static async Task<List<string>> GetUserModuleIdsAsync(UserInfo user)
    {
        if (CoreConfig.OnRoleModule == null)
            return [];

        var key = string.Format(ModuleIdsCacheKey, user.Id);
        var cached = Cache.Get<List<string>>(key);
        if (cached != null)
            return cached;

        List<string> moduleIds;
        using (var db = Database.Create())
        {
            db.User = user;
            moduleIds = await CoreConfig.OnRoleModule(db, user.Id) ?? [];
        }

        Cache.Set(key, moduleIds, ModuleIdsCacheTime);
        return moduleIds;
    }
}