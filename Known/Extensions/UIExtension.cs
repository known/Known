using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Extensions;

public static class UIExtension
{
    #region Form
    public static void FormPage(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page", child);
    public static void FormPageButton(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-page-button", child);
    internal static void FormButton(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-button", child);
    public static void FormAction(this RenderTreeBuilder builder, Action child) => builder.Div("kui-form-action", child);
    #endregion

    #region Page
    internal static void BuildTablePage<TItem>(this RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Component<TablePage<TItem>>().Set(c => c.Model, model).Build();
    }

    internal static void Build404Page(this IUIService service, RenderTreeBuilder builder, string pageId)
    {
        service.BuildResult(builder, "404", $"{service.Language["Tip.Page404"]}PageId={pageId}");
    }
    #endregion

    #region Tag
    public static void Tag(this RenderTreeBuilder builder, string text)
    {
        builder.Component<KTag>().Set(c => c.Text, text).Build();
    }
    #endregion

    #region Icon
    public static void Icon(this IUIService service, RenderTreeBuilder builder, string icon, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (string.IsNullOrWhiteSpace(icon))
            return;

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