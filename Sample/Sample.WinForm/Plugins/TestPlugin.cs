using Known.Plugins;
using Microsoft.AspNetCore.Components.Rendering;

namespace Sample.WinForm.Plugins;

[PagePlugin("测试", "file")]
class TestPlugin : PluginBase<TestPluginInfo>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Draggable = true;
        Actions.Add(new ActionInfo { Id = "Setting", Icon = "setting", Name = "设置" });
    }

    protected override void BuildPlugin(RenderTreeBuilder builder)
    {
        builder.Ul(() =>
        {
            builder.Li("", $"当前页面：{Context?.Current?.Name}");
            builder.Li("", $"插件ID：{Plugin?.Id}");
            builder.Li("", $"插件类型：{Plugin?.Type}");
            builder.Li("", $"插件参数：{Parameter?.Name}");
        });
    }
}

public class TestPluginInfo
{
    public string Name { get; set; }
}