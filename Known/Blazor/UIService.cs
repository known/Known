using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public interface IUIService
{
    Type GetInputType(ColumnInfo column);
    void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new();
    void Toast(string message, StyleType style = StyleType.Success);
    void Alert(string message);
    void Confirm(string message, Func<Task> action);
    void ShowModal(ModalOption option);
    void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new();
    void BuildForm<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new();
    void BuildPage(RenderTreeBuilder builder, PageModel model);
    void BuildPage<TItem>(RenderTreeBuilder builder, TablePageModel<TItem> model) where TItem : class, new();
    void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new();
    void BuildTree(RenderTreeBuilder builder, TreeModel model);
    void BuildSteps(RenderTreeBuilder builder, StepModel model);
    void BuildTabs(RenderTreeBuilder builder, TabModel model);
    void BuildTag(RenderTreeBuilder builder, string text, string color);
    void BuildIcon(RenderTreeBuilder builder, string type);
    void BuildResult(RenderTreeBuilder builder, string status, string message);
    void BuildButton(RenderTreeBuilder builder, ActionInfo info);
    void BuildInput<TValue>(RenderTreeBuilder builder, InputOption<TValue> option);
    void BuildCheckList(RenderTreeBuilder builder, ListOption<string[]> option);
}