using Known.Studio.Services;

namespace Known.Studio.Pages;

class DevCode : BasePage
{
    private string domain;
    private string curItem = "Entity";
    private string codeString;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        BuildLeft(builder);
        BuildRight(builder);
    }

    private void BuildLeft(RenderTreeBuilder builder)
    {
        builder.Div("dc-left", attr =>
        {
            builder.Pre("tips", @"说明：
工程：名称|表前缀
实体：名称|代码
字段：名称|代码|类型|长度|必填|查询
字段类型：CheckBox,CheckList,Date,Number,RadioList,Select,Text,TextArea</pre>
<pre class=""demo"">示例：
Demo|Dm
测试|Test
文本|Field1|Text|50|Y|Y
日期|Field2|Date");
            builder.Field<TextArea>("领域模型", "Domain").ValueChanged(v => domain = v)
                   .Set(f => f.Height, 320)
                   .Build();
            builder.Div("dc-btn", attr =>
            {
                builder.Button("生成", "fa fa-download", Callback(OnGenerate));
            });
        });
    }

    private void BuildRight(RenderTreeBuilder builder)
    {
        builder.Div("dc-right", attr =>
        {
            builder.Component<Tab>()
                   .Set(c => c.Codes, "SQL,Entity,Service,ListCS,FormCS")
                   .Set(c => c.OnChanged, OnTabChanged)
                   .Build();
            builder.Element("pre", attr => attr.Id("code").Class("code prettyprint source linenums"));
        });
    }

    private async void OnGenerate()
    {
        codeString = CodeService.GetCode(curItem, domain);
        await JS.InvokeAsync<string>("printCode", new object[] { codeString });
    }

    private async void OnTabChanged(MenuItem item)
    {
        curItem = item.Code;
        codeString = CodeService.GetCode(item.Code, domain);
        await JS.InvokeAsync<string>("printCode", new object[] { codeString });
    }
}