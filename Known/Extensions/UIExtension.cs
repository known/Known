using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Extensions;

public static class UIExtension
{
    #region Internal
    internal static void FormPage(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page", child);
    internal static void FormPageButton(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page-button", child);
    internal static void FormAction(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-action", child);

    internal static void BuildTablePage<TItem>(this RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Component<TablePage<TItem>>().Set(c => c.Model, model).Build();
    }
    #endregion

    #region Icon
    public static void Icon(this IUIService service, RenderTreeBuilder builder, string icon, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (icon.StartsWith("fa"))
        {
            builder.Span(icon, "", onClick);
            return;
        }

        service.BuildIcon(builder, icon, onClick);
    }
    #endregion

    #region Box
    public static void GroupBox(this RenderTreeBuilder builder, string title, Action child)
    {
        builder.Div("kui-group-box", () =>
        {
            builder.Label().Class("legend").Text(title).Close();
            builder.Div("body", child);
        });
    }
    #endregion

    #region Button
    public static void Button(this IUIService service, RenderTreeBuilder builder, ActionInfo action, EventCallback<MouseEventArgs> onClick)
    {
        action.OnClick = onClick;
        service.BuildButton(builder, action);
    }

    public static void Button(this IUIService service, RenderTreeBuilder builder, string name, EventCallback<MouseEventArgs> onClick, string style = null)
    {
        service.BuildButton(builder, new ActionInfo
        {
            Name = name,
            OnClick = onClick,
            Style = style
        });
    }
    #endregion

    #region Toast
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
    #endregion
}