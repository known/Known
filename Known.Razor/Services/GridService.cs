namespace Known.Razor;

partial class UIService
{
    public void ToggleQuery(string id) => InvokeVoidAsync("KR_toggleQuery", id);
    public void FixedTable(string id) => InvokeVoidAsync("KR_fixedTable", id);
}