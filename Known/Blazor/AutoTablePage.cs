using Known.Designers;
using Known.Entities;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.Blazor;

class AutoTablePage : BaseTablePage<Dictionary<string, object>>
{
    private bool isEditPage;
    private string pageId;
    private string TableName { get; set; }

    public override async Task RefreshAsync()
    {
        if (isEditPage)
            InitTable();

        await base.RefreshAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (pageId != PageId)
        {
            pageId = PageId;
            InitTable();
            await Table.RefreshAsync();
        }
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        base.BuildPage(builder);
        if (CurrentUser != null && CurrentUser.UserName == "admin")
        {
            builder.Div("kui-page-designer", () =>
            {
                builder.Icon("fa fa-edit", this.Callback<MouseEventArgs>(OnEditPage));
            });
        }
    }

    public void New() => Table.NewForm(m => Platform.Auto.SaveModelAsync(TableName, m), []);
    public void DeleteM() => Table.DeleteM(m => Platform.Auto.DeleteModelsAsync(TableName, m));
    public void Edit(Dictionary<string, object> row) => Table.EditForm(m => Platform.Auto.SaveModelAsync(TableName, m), row);
    public void Delete(Dictionary<string, object> row) => Table.Delete(m => Platform.Auto.DeleteModelsAsync(TableName, m), row);
    public void Import() => ShowImportForm(TableName);
    public async void Export() => await ExportDataAsync(ExportMode.Query);

    private void InitTable()
    {
        InitMenu();
        Table.SetPage(this);
        TableName = DataHelper.GetEntity(Context.Module?.EntityData)?.Id;
        Table.OnQuery = c => Platform.Auto.QueryModelsAsync(TableName, c);
        Table.Criteria.Clear();
    }

    private void OnEditPage(MouseEventArgs args)
    {
        isEditPage = true;
        var form = new FormModel<SysModule>(this)
        {
            Data = Context.Module,
            OnSave = Platform.Module.SaveModuleAsync,
            OnSaved = data => Context.Module = data
        };
        var model = new DialogModel
        {
            Title = $"{Language["Designer.EditPage"]} - {PageName}",
            Maximizable = true,
            DefaultMaximized = true,
            Width = 1200,
            Content = b => b.Component<SysModuleForm>().Set(c => c.Model, form).Set(c => c.IsPageEdit, true).Build()
        };
        UI.ShowDialog(model);
        form.OnClose = async () =>
        {
            isEditPage = false;
            await model.OnClose?.Invoke();
        };
    }
}