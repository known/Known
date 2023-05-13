namespace Known.Razor;

partial class UIService
{
    public async ValueTask<string> ExcelImport(Stream stream)
    {
        if (stream == null)
            return null;

        var module = await moduleTask.Value;
        using var streamRef = new DotNetStreamReference(stream);
        return await module.InvokeAsync<string>("KR_excelImport", streamRef);
    }

    public void ExcelExport(string fileName, List<string[]> datas) => InvokeVoidAsync("KR_excelExport", fileName, datas);
}