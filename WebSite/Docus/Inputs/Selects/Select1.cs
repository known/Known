namespace WebSite.Docus.Inputs.Selects;

class Select1 : BaseComponent
{
    private readonly string Codes = "孙膑,后羿,妲己";
    private readonly CodeInfo[] Items = new CodeInfo[]
    {
        new CodeInfo("1", "辅助"),
        new CodeInfo("2", "射手"),
        new CodeInfo("3", "法师")
    };

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //Codes属性设置列表项
        //该值为数据字典类别或逗号分割的字符串
        builder.Field<KSelect>("示例1：", "Select1").Set(f => f.Codes, Codes).Build();
        //Items属性设置列表项
        builder.Field<KSelect>("示例2：", "Select2").Set(f => f.Items, Items).Build();
        //赋值
        builder.Field<KSelect>("示例3：", "Select3").Value("孙膑").Set(f => f.Codes, Codes).Build();
        builder.Field<KSelect>("示例4：", "Select4").Value("3").Set(f => f.Items, Items).Build();
    }
}