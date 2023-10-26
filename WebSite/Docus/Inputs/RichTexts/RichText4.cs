namespace WebSite.Docus.Inputs.RichTexts;

class RichText4 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<KRichText>("内容：", "RichText4")
               .Set(f => f.Option, new
               {
                   Height = 200,
                   Placeholder = "请输入通知内容"
               })
               .Set(f => f.Storage, new StorageOption
               {
                   Type = StorageType.Server,//默认本地服务器，支持阿里云OSS和腾讯云COS
                   KeyId = "accessKeyId",
                   KeySecret = "accessKeySecret",
                   Region = "your region",
                   Bucket = "your bucket name"
               })
               .Build();
    }
}