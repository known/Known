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
        var option = new DrawerOptions
        {
            Title = Language?[model.Title],
            Width = model.Width,
            Closable = true,
            WrapClassName = model.ClassName,
            Placement = DrawerPlacement.Right,
            MaskClosable = true,
            Content = model.Content
        };
        drawer.CreateAsync(option);
    }
}