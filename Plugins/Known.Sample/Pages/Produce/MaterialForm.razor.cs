namespace Known.Sample.Pages.Produce;

public partial class MaterialForm
{
    private IProduceService Service;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IProduceService>();
        Model.OnSaving = d =>
        {
            d.PackFields = ListItems;
            return Task.FromResult(true);
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var data = await Service.GetMaterialAsync(Model.Data.Id);
            Model.Data = data;
            ListItems.AddRange(data.PackFields ?? []);
            StateChanged();
        }
    }

    private Dictionary<string, object> GetLabelData()
    {
        var data = new Dictionary<string, object>();
        foreach (var item in ListItems)
        {
            if (!string.IsNullOrWhiteSpace(item.Name))
                data[item.Name] = "未填写";
        }
        return data;
    }
}