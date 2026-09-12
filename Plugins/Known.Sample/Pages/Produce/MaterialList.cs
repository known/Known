namespace Known.Sample.Pages.Produce;

[Route("/pms/materials")]
[Menu(AppConstant.Produce, "客户料号", "bars", 1)]
public class MaterialList : BaseTablePage<TbMaterial>
{
    private IProduceService Service;

    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IProduceService>();

        Table.FormType = typeof(MaterialForm);
        Table.Form = new FormInfo { Width = 900 };
        Table.OnQuery = Service.QueryMaterialsAsync;

        Table.AddColumn(c => c.Name, true);
        //Table.Columns.Add(new ColumnInfo
        //{
        //    Id = "Name",
        //    Name = "名称",
        //    IsQuery = true // 这样添加查询条件不显示
        //});
    }

    [Action] public void New() => Table.NewForm(Service.SaveMaterialAsync, new TbMaterial());
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteMaterialsAsync);
    [Action] public void Edit(TbMaterial row) => Table.EditForm(Service.SaveMaterialAsync, row);
    [Action] public void Delete(TbMaterial row) => Table.Delete(Service.DeleteMaterialsAsync, row);
}