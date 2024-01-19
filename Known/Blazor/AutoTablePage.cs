using Known.Designers;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

class AutoTablePage : BaseTablePage<Dictionary<string, object>>
{
    private string TableName { get; set; }

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        TableName = DataHelper.GetEntity(Module?.EntityData)?.Id;
        Table.OnQuery = c => Platform.Auto.QueryModelsAsync(TableName, c);
    }

    [Action] public void New() => Table.NewForm(m => Platform.Auto.SaveModelAsync(TableName, m), []);
    [Action] public void DeleteM() => Table.DeleteM(m => Platform.Auto.DeleteModelsAsync(TableName, m));
    [Action] public void Edit(Dictionary<string, object> row) => Table.EditForm(m => Platform.Auto.SaveModelAsync(TableName, m), row);
    [Action] public void Delete(Dictionary<string, object> row) => Table.Delete(m => Platform.Auto.DeleteModelsAsync(TableName, m), row);
    [Action] public void Import() => ShowImportForm(TableName);
    [Action] public async void Export() => await ExportDataAsync(ExportMode.Query);
}