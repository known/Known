using AntDesign;

namespace Known.Blazor;

public partial class UIService
{
    /// <summary>
    /// 显示抽屉弹窗。
    /// </summary>
    /// <param name="model">抽屉配置模型。</param>
    /// <returns></returns>
    public void ShowDrawer(DrawerModel model)
    {
        var className = CssBuilder.Default("kui-drawer").AddClass(model.ClassName).BuildClass();
        var option = new DrawerOptions
        {
            Title = Language?[model.Title],
            Width = model.Width,
            Closable = true,
            WrapClassName = className,
            Placement = DrawerPlacement.Right,
            MaskClosable = true,
            Content = GetDrawerContent(model)
        };
        drawer.CreateAsync(option);
    }

    private RenderFragment GetDrawerContent(DrawerModel model)
    {
        return b => b.BuildBody(Context, model.Content);
    }
}