namespace Known.Blazor;

public interface IUIService
{
    Language Language { get; set; }
    Type GetInputType(Type dataType, FieldType fieldType);
    void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new();
    Task Toast(string message, StyleType style = StyleType.Success);
    Task Notice(string message, StyleType style = StyleType.Success);
    void Alert(string message, Func<Task> action = null);
    void Confirm(string message, Func<Task> action);
    void ShowDialog(DialogModel model);
    void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new();
    void BuildForm<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new();
    void BuildToolbar(RenderTreeBuilder builder, ToolbarModel model);
    void BuildQuery(RenderTreeBuilder builder, TableModel model);
    void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new();
    void BuildTree(RenderTreeBuilder builder, TreeModel model);
    void BuildSteps(RenderTreeBuilder builder, StepModel model);
    void BuildTabs(RenderTreeBuilder builder, TabModel model);
    void BuildTag(RenderTreeBuilder builder, string text, string color = null);
    void BuildIcon(RenderTreeBuilder builder, string type, EventCallback<MouseEventArgs>? onClick = null);
    void BuildResult(RenderTreeBuilder builder, string status, string message);
    void BuildButton(RenderTreeBuilder builder, ActionInfo info);
    void BuildSearch(RenderTreeBuilder builder, InputModel<string> model);
    void BuildText(RenderTreeBuilder builder, InputModel<string> model);
    void BuildTextArea(RenderTreeBuilder builder, InputModel<string> model);
    void BuildPassword(RenderTreeBuilder builder, InputModel<string> model);
    void BuildDatePicker<TValue>(RenderTreeBuilder builder, InputModel<TValue> model);
    void BuildNumber<TValue>(RenderTreeBuilder builder, InputModel<TValue> model);
    void BuildCheckBox(RenderTreeBuilder builder, InputModel<bool> model);
    void BuildSwitch(RenderTreeBuilder builder, InputModel<bool> model);
    void BuildSelect(RenderTreeBuilder builder, InputModel<string> model);
    void BuildRadioList(RenderTreeBuilder builder, InputModel<string> model);
    void BuildCheckList(RenderTreeBuilder builder, InputModel<string[]> model);
}