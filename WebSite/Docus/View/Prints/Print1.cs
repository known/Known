namespace WebSite.Docus.View.Prints;

class Print1 : BaseComponent
{
    [Parameter] public bool IsPrint { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildStyle(builder);  //建造打印样式
        BuildForm(builder);   //建造打印表单
        BuildButton(builder); //建造打印按钮，打印时不显示
    }

    private static void BuildStyle(RenderTreeBuilder builder)
    {
        builder.Markup(@"<style>
.demo-print .txt-right {text-align:right;}
.demo-print .field {display:grid;grid-template-columns:repeat(auto-fit,minmax(0%,1fr));grid-column-gap:10px;margin:10px 0;}
.demo-print .title {font-size:1.5rem;font-weight:bold;text-align:center;}
.demo-print .content {height:100px;border-top:2px solid #ccc;;border-bottom:1px solid #ccc;padding:10px;}
</style>");
    }

    private void BuildForm(RenderTreeBuilder builder)
    {
        builder.Div("demo-print", attr =>
        {
            builder.Div("title", "XXX公司采购申请单");
            builder.Div("field", attr =>
            {
                builder.Div("", "申请部门：XXX");
                builder.Div("txt-right", $"申请日期：{DateTime.Now:yyyy-MM-dd}");
            });
            builder.Div("content", "这里是采购单信息");
            builder.Div("field", attr =>
            {
                builder.Div("", "申请人：XXX");
                builder.Div("", "审核人：XXX");
                builder.Div("", "采购人：XXX");
            });
        });
    }

    private void BuildButton(RenderTreeBuilder builder)
    {
        if (IsPrint)
            return;

        builder.Button(ToolButton.Print, Callback(OnPrint));
    }

    private void OnPrint() => UI.Print<Print1>(form => form.Set(c => c.IsPrint, true));
}