namespace Known.Razor;

partial class UIService
{
    public void FixedTable(string id) => InvokeVoidAsync("KRazor.fixedTable", id);
}