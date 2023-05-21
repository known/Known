namespace Known.Razor;

partial class UIService
{
    public void ToggleQuery(string id) => InvokeVoidAsync("KRazor.toggleQuery", id);
    public void FixedTable(string id) => InvokeVoidAsync("KRazor.fixedTable", id);
}