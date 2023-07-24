using WebSite.Docus.Nav.Toolbars;

namespace WebSite.Docus.Nav;

class KToolbar : BaseDocu
{
    internal static readonly List<ButtonInfo> Tools = new()
    {
        new ButtonInfo("Load", "加载", "fa fa-refresh", StyleType.Default),
        new ButtonInfo("View", "只读", "fa fa-file-text-o", StyleType.Warning),
        new ButtonInfo("Edit", "编辑", "fa fa-file-o", StyleType.Success),
        new ButtonInfo("Check", "验证", "fa fa-check", StyleType.Info),
        new ButtonInfo("Save", "保存", "fa fa-save", StyleType.Primary),
        new ButtonInfo("Clear", "清空", "fa fa-trash-o", StyleType.Danger),
        new ButtonInfo("Clear", "禁用", "fa fa-trash-o", StyleType.Primary) { Enabled = false }
    };

    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "该组件用于显示一组按钮，可用于列表操作",
            "可在外部设置按钮可见和可用"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo("公用代码", @"//定义按钮组
internal static readonly List<ButtonInfo> Tools = new()
{
    new ButtonInfo(""Load"", ""加载"", ""fa fa-refresh"", StyleType.Default),
    new ButtonInfo(""View"", ""只读"", ""fa fa-file-text-o"", StyleType.Warning),
    new ButtonInfo(""Edit"", ""编辑"", ""fa fa-file-o"", StyleType.Success),
    new ButtonInfo(""Check"", ""验证"", ""fa fa-check"", StyleType.Info),
    new ButtonInfo(""Save"", ""保存"", ""fa fa-save"", StyleType.Primary),
    new ButtonInfo(""Clear"", ""清空"", ""fa fa-trash-o"", StyleType.Danger),
    new ButtonInfo(""Clear"", ""禁用"", ""fa fa-trash-o"", StyleType.Primary) { Enabled = false }
};");

        builder.BuildDemo<Toolbar1>();
        builder.BuildDemo<Toolbar2>();
        builder.BuildDemo<Toolbar3>();
        builder.BuildDemo<Toolbar4>();
    }
}