namespace WebSite.Docus.Inputs.Pickers;

class Picker3 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Picker>("客户：", "Picker3")
               .Set(f => f.TextField, "Picker3Name")
               .Set(f => f.Pick, new CustomerList("#"))
               .Set(f => f.PickSeparator, "#")
               .Build();
    }
}