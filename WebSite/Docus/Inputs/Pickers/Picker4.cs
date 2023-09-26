namespace WebSite.Docus.Inputs.Pickers;

class Picker4 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Picker>("客户：", "Picker4")
               .Set(f => f.TextField, "Picker4Name")
               .Set(f => f.Pick, new CustomerList())
               .Set(f => f.PickIdKey, nameof(KmCustomer.Code))
               .Set(f => f.PickTextKey, nameof(KmCustomer.Name))
               .Build();
    }
}