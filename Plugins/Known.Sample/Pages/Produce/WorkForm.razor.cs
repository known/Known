namespace Known.Sample.Pages.Produce;

public partial class WorkForm
{
    private IProduceService Service;
    private DynamicFormModel Pack;

    protected override async Task OnInitFormAsync()
    {
        await base.OnInitFormAsync();
        Service = await CreateServiceAsync<IProduceService>();
        Model.ConfirmText = "确定要开始生产该工单？";
        if (Model.IsNew)
            Model.OnSavedAsync = row => JS.PrintWorkAsync(row, CurrentUser.UserName);
        Pack = new DynamicFormModel(this) { SmallLabel = true };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var data = await Service.GetWorkAsync(Model.Data.Id);
            Model.Data = data;
            Pack.IsView = Model.IsView;
            Pack.Data = data.PackInfo;
            Pack.SetPackForm(data.PackFields);
            StateChanged();
        }
    }

    private void OnCustGNoChanged(TbMaterial info)
    {
        Model.Data.CustGNo = info?.CustGNo;
        Model.Data.PackFields = info?.PackFields;
        Model.Data.SetPackInfo();
        Pack.Data = Model.Data.PackInfo;
        Pack.SetPackForm(info?.PackFields);
    }
}