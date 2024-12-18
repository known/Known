using AntDesign;

namespace Known.Internals;

/// <summary>
/// 编辑模式编辑菜单图标按钮组件。
/// </summary>
public class EditMenu : BaseComponent
{
    private TreeModel tree;
    private FormModel<MenuInfo> form;

    /// <summary>
    /// 取得或设置模块信息列表。
    /// </summary>
    [Parameter] public List<MenuInfo> Data { get; set; }

    /// <summary>
    /// 取得或设置菜单编辑后调用委托。
    /// </summary>
    [Parameter] public Action<List<MenuInfo>> OnEdit { get; set; }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        tree = new TreeModel
        {
            Data = Data.ToMenuItems(true),
            OnNodeClick = m => form.LoadDataAsync(m)
        };
        form = new FormModel<MenuInfo>(this) { SmallLabel = true, Data = new MenuInfo() };
        form.AddRow().AddColumn(c => c.Name, c => c.Required = true);
        form.AddRow().AddColumn(c => c.Icon, c =>
        {
            c.Required = true;
            c.Type = FieldType.Custom;
            c.CustomField = nameof(IconPicker);
        });
        form.AddRow().AddColumn(c => c.Url);
    }

    /// <summary>
    /// 呈现菜单编辑图标组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Class("kui-edit").OnClick(this.Callback<MouseEventArgs>(OnClick)).Child(() =>
        {
            builder.Component<KIcon>().Set(c => c.Title, "编辑菜单").Set(c => c.Icon, "menu").Build();
        });
    }

    private void OnClick(MouseEventArgs e)
    {
        var model = new DialogModel();
        model.Title = "编辑菜单";
        model.Width = 600;
        model.Content = BuildDialog;
        model.OnOk = () =>
        {
            OnEdit?.Invoke(Data);
            return model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    private void BuildDialog(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-37", () =>
        {
            builder.Component<AntTree>()
                   .Set(c => c.Model, tree)
                   .Set(c => c.Draggable, true)
                   .Set(c => c.OnDrop, this.Callback<TreeEventArgs<MenuInfo>>(OnDrop))
                   .Set(c => c.OnDragEnd, this.Callback<TreeEventArgs<MenuInfo>>(OnDragEnd))
                   .Build();
            builder.Form(form);
        });
    }

    private Task OnDrop(TreeEventArgs<MenuInfo> args)
    {
        return Task.CompletedTask;
    }

    private Task OnDragEnd(TreeEventArgs<MenuInfo> args)
    {
        return Task.CompletedTask;
    }
}