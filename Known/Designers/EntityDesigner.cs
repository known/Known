using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityDesigner : BaseComponent
{
    [Parameter] public string Model { get; set; }
    [Parameter] public Action<string> OnChanged { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading(this, this.BuildTree(b =>
        {
            b.Div("kui-designer entity", () =>
            {
                b.Div("panel-model", () =>
                {
                    b.Div("title", "实体模型");
                    b.Markup(@"<pre>说明：
实体：名称|代码
字段：名称|代码|类型|长度|必填|查询
字段类型：CheckBox,CheckList,Date,Number,RadioList,Select,Text,TextArea
示例：
测试|KmTest
文本|Field1|Text|50|Y|Y
数值|Field2|Number|18,5
日期|Field3|Date</pre>");
                    UI.BuildTextArea(b, new InputModel<string>
                    {
                        Value = Model,
                        ValueChanged = this.Callback(OnChanged)
                    });
                });
                b.Div("panel-view", () =>
                {
                    b.Component<EntityView>().Build();
                });
            });
        }));
    }
}