namespace Known.Razor;

partial class UIService
{
    internal void InitTable(string id) => InvokeVoidAsync("KRazor.initTable", id);
    internal void SetTable(string id) => InvokeVoidAsync("KRazor.setTable", id);
    internal void FixedTable(string id) => InvokeVoidAsync("KRazor.fixedTable", id);
}