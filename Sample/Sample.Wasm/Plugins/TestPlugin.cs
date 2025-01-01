namespace Sample.Wasm.Plugins;

[PagePlugin("测试", "file", Category = nameof(PagePluginType.Other), Sort = 1)]
class TestPlugin : PluginBase<TestPluginInfo>
{
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Draggable = true;
        AddAction("setting", "设置", OnSetting);
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

    private void OnSetting()
    {
        var model = new FormModel<TestPluginInfo>(this, true)
        {
            Title = "测试插件配置",
            Data = Parameter ?? new TestPluginInfo(),
            OnSave = SaveParameterAsync,
            OnSaved = d => StateChanged()
        };
        UI.ShowForm(model);
    }
}

public class TestPluginInfo
{
    [Form]
    [Required]
    public string Name { get; set; }
}