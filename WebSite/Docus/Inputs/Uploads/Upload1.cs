namespace WebSite.Docus.Inputs.Uploads;

class Upload1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KUpload>("附件1：", "Upload1").Build();
        builder.Field<KUpload>("附件2：", "Upload2")
               .Set(f => f.IsMultiple, true)
               .Build();
        builder.Field<KUpload>("附件3：", "Upload3")
               .Set(f => f.Accept, "image/jpeg,image/png")
               .Build();
        builder.Field<KUpload>("附件4：", "Upload4")
               .Set(f => f.IsButton, true)
               .Set(f => f.ButtonText, "导入附件")
               .Build();
    }
}