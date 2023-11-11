using Known.Razor;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Extensions;

public static class UIExtension
{
    public static void Info(this IUIService service, string message) => service.Toast(message, StyleType.Info);
    public static void Warning(this IUIService service, string message) => service.Toast(message, StyleType.Warning);
    public static void Error(this IUIService service, string message) => service.Toast(message, StyleType.Error);

    public static void Result(this IUIService service, Result result, Action onSuccess = null)
    {
        if (!result.IsValid)
        {
            service.Error(result.Message);
            return;
        }

        onSuccess?.Invoke();
        service.Toast(result.Message);
    }

    public static void Prompt(this IUIService service, string title, Action<RenderTreeBuilder> content, Action<dynamic> action)
    {
        //Show<PromptForm>(title, size, action: attr =>
        //{
        //    attr.Set(c => c.InDialog, true)
        //        .Set(c => c.Content, content)
        //        .Set(c => c.Action, action);
        //});
    }
}