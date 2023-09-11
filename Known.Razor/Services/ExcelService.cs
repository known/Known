namespace Known.Razor;

partial class UIService
{
    internal async ValueTask<string> ExcelImport(Stream stream)
    {
        if (stream == null)
            return null;

        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        return await module.InvokeAsync<string>("KRazor.excelImport", streamRef);
    }

    internal void ExcelExport(string fileName, List<string[]> datas) => InvokeVoidAsync("KRazor.excelExport", fileName, datas);
}