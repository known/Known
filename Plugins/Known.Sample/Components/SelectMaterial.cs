namespace Known.Sample.Components;

public class SelectMaterial : AntDropdownTable<TbMaterial>
{
    private IProduceService Service;

    protected override Func<TbMaterial, string> OnValue => d => d.CustGNo;

    protected override async Task OnInitializeAsync()
    {
        await base.OnInitializeAsync();
        Service = await CreateServiceAsync<IProduceService>();

        Table.OnQuery = Service.QueryMaterialsAsync;
        Table.AddColumn(c => c.CustGNo, true).ViewLink(false);
        Table.AddColumn(c => c.Note);
    }
}