namespace Known.Razor;

partial class UIService
{
    internal void InitTable(string id) => InvokeVoidAsync("KRazor.initTable", id);
    internal void SetTableTop(string id) => InvokeVoidAsync("KRazor.setTableTop", id);
    internal void FixedTable(string id) => InvokeVoidAsync("KRazor.fixedTable", id);
}