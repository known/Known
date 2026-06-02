namespace Known.MyModule.Pages;

[Route("/demo/items")]
[Menu("MyModule", "示例项目", "bars", 1)]
public class DemoItemList : BaseTablePage<TbDemoItem>
{
    private IDemoItemService Service;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IDemoItemService>();

        Table.OnQuery = Service.QueryItemsAsync;
    }

    [Action]
    public void New() => Table.NewForm(Service.SaveItemAsync, new TbDemoItem());

    [Action]
    public void Edit(TbDemoItem row) => Table.EditForm(Service.SaveItemAsync, row);

    [Action]
    public void Delete(TbDemoItem row) => Table.Delete(Service.DeleteItemsAsync, row);

    [Action]
    public void DeleteM() => Table.DeleteM(Service.DeleteItemsAsync);
}
