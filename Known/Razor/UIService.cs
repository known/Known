using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public interface IUIService
{
    Type GetInputType(ColumnAttribute column);
    void Toast(string message, StyleType style = StyleType.Success);
    void Alert(string message);
    void Confirm(string message, Func<Task> action);
    void ShowModal(ModalOption option);
    void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new();
    void BuildPage<TItem>(RenderTreeBuilder builder, PageModel<TItem> model) where TItem : class, new();
    void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new();
    void BuildTree(RenderTreeBuilder builder, TreeModel model);
    void BuildTag(RenderTreeBuilder builder, string text, string color);
    void BuildResult(RenderTreeBuilder builder, string status, string message);
    void BuildButton(RenderTreeBuilder builder, ButtonOption option);
    void BuildInput<TValue>(RenderTreeBuilder builder, InputOption<TValue> option);
}