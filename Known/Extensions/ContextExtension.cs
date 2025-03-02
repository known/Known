namespace Known.Extensions;

/// <summary>
/// 上下文扩展类。
/// </summary>
public static class ContextExtension
{
    /// <summary>
    /// 导航到指定动作对应的页面。
    /// </summary>
    /// <param name="context">UI上下文。</param>
    /// <param name="item">跳转的动作对象。</param>
    public static void NavigateTo(this UIContext context, ActionInfo item)
    {
        if (item == null)
            return;

        var menu = new MenuInfo { Id = item.Id, Name = item.Name, Icon = item.Icon, Url = item.Url };
        context.NavigateTo(menu);
    }

    /// <summary>
    /// 调用对象指定操作信息的方法。
    /// </summary>
    /// <param name="context">UI上下文。</param>
    /// <param name="sender">触发操作的对象。</param>
    /// <param name="info">操作信息。</param>
    /// <param name="parameters">操作参数。</param>
    public static void OnAction(this UIContext context, object sender, ActionInfo info, object[] parameters)
    {
        var type = sender.GetType();
        var paramTypes = parameters?.Select(p => p.GetType()).ToArray();
        var method = paramTypes == null
                   ? GetMethod(type, info.Id)
                   : type.GetMethod(info.Id, paramTypes);
        if (method == null)
        {
            var message = context.Language["Tip.NoMethod"].Replace("{method}", $"{info.Name}[{type.Name}.{info.Id}]");
            context.UI.Error(message);
            return;
        }
        try
        {
            method.Invoke(sender, parameters);
        }
        catch (Exception ex)
        {
            Logger.Error(LogTarget.FrontEnd, context.CurrentUser, ex.ToString());
            context.UI.Error(ex.Message);
        }
    }

    private static MethodInfo GetMethod(Type type, string name)
    {
        var methods = type.GetMethods();
        return methods.Where(m => m.Name == name && m.GetParameters()?.Length == 0).FirstOrDefault();
    }
}