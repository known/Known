using Known;

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

        builder.BuildDemo<Toolbar1>("默认示例", @"class Toolbar1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Toolbar>()
               .Set(c => c.Tools, Tools)
               .Set(c => c.OnAction, OnAction)
               .Build();
    }

    public void Load() => UI.Toast(""点击了加载按钮！"");
    public void Edit() => UI.Toast(""点击了编辑按钮！"");
    public void Save() => UI.Toast(""点击了保存按钮！"");
    public void Clear() => UI.Toast(""点击了清空按钮！"");

    private void OnAction(ButtonInfo info)
    {
        var method = GetType().GetMethod(info.Id);
        if (method == null)
            UI.Toast($""{info.Name}方法不存在！"", StyleType.Danger);
        else
            method.Invoke(this, null);
        StateChanged();
    }
}");

        builder.BuildDemo<Toolbar2>("下拉按钮示例", @"class Toolbar2 : BaseComponent
{
    private readonly List<ButtonInfo> tools = new();

    public Toolbar2()
    {
        tools.Add(new ButtonInfo(""Edit"", ""编辑"", ""fa fa-file-o"", StyleType.Success));
        tools.Add(new ButtonInfo(""Clear"", ""清空"", ""fa fa-trash-o"", StyleType.Danger));
        var more = new ButtonInfo(""More"", ""更多"", """");
        more.Children.Add(new ButtonInfo(""More1"", ""操作一"", ""fa fa-close""));
        more.Children.Add(new ButtonInfo(""More2"", ""操作二"", ""fa fa-user"") { Enabled = false });
        more.Children.Add(new ButtonInfo(""More3"", ""操作三"", ""fa fa-cog""));
        tools.Add(more);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Toolbar>()
                .Set(c => c.Tools, tools)
                .Set(c => c.OnAction, OnAction)
                .Build();
    }

    private void OnAction(ButtonInfo info)
    {
        var method = GetType().GetMethod(info.Id);
        if (method == null)
            UI.Toast($""{info.Name}方法不存在！"", StyleType.Danger);
        else
            method.Invoke(this, null);
        StateChanged();
    }
}");

        builder.BuildDemo<Toolbar3>("按钮可见示例", @"class Toolbar3 : BaseComponent
{
    private Toolbar? toolbar;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Toolbar>()
               .Set(c => c.Tools, Tools)
               .Build(value => toolbar = value);

        builder.Field<CheckBox>(""ToolVisible"").Value(""True"").ValueChanged(OnToolVisibleChanged).Set(f => f.Text, ""编辑按钮可见"").Build();
    }

    private void OnToolVisibleChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar?.SetItemVisible(check, ""Edit"");
    }
}");

        builder.BuildDemo<Toolbar4>("按钮可用示例", @"class Toolbar4 : BaseComponent
{
    private Toolbar? toolbar;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Component<Toolbar>()
               .Set(c => c.Tools, Tools)
               .Build(value => toolbar = value);

        builder.Field<CheckBox>(""ToolEnabled"").Value(""True"").ValueChanged(OnToolEnabledChanged).Set(f => f.Text, ""验证、保存按钮可用"").Build();
    }

    private void OnToolEnabledChanged(string value)
    {
        var check = Utils.ConvertTo<bool>(value);
        toolbar?.SetItemEnabled(check, ""Check"", ""Save"");
    }
}");
    }

    class Toolbar1 : BaseComponent
    {
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.Component<Toolbar>()
                   .Set(c => c.Tools, Tools)
                   .Set(c => c.OnAction, OnAction)
                   .Build();
        }

        public void Load() => UI.Toast("点击了加载按钮！");
        public void Edit() => UI.Toast("点击了编辑按钮！");
        public void Save() => UI.Toast("点击了保存按钮！");
        public void Clear() => UI.Toast("点击了清空按钮！");

        private void OnAction(ButtonInfo info)
        {
            var method = GetType().GetMethod(info.Id);
            if (method == null)
                UI.Toast($"{info.Name}方法不存在！", StyleType.Danger);
            else
                method.Invoke(this, null);
            StateChanged();
        }
    }

    class Toolbar2 : BaseComponent
    {
        private readonly List<ButtonInfo> tools = new();

        public Toolbar2()
        {
            tools.Add(new ButtonInfo("Edit", "编辑", "fa fa-file-o", StyleType.Success));
            tools.Add(new ButtonInfo("Clear", "清空", "fa fa-trash-o", StyleType.Danger));
            var more = new ButtonInfo("More", "更多", "");
            more.Children.Add(new ButtonInfo("More1", "操作一", "fa fa-close"));
            more.Children.Add(new ButtonInfo("More2", "操作二", "fa fa-user") { Enabled = false });
            more.Children.Add(new ButtonInfo("More3", "操作三", "fa fa-cog"));
            tools.Add(more);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.Component<Toolbar>()
                   .Set(c => c.Tools, tools)
                   .Set(c => c.OnAction, OnAction)
                   .Build();
        }

        private void OnAction(ButtonInfo info)
        {
            var method = GetType().GetMethod(info.Id);
            if (method == null)
                UI.Toast($"{info.Name}方法不存在！", StyleType.Danger);
            else
                method.Invoke(this, null);
            StateChanged();
        }
    }

    class Toolbar3 : BaseComponent
    {
        private Toolbar? toolbar;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.Component<Toolbar>()
                   .Set(c => c.Tools, Tools)
                   .Build(value => toolbar = value);

            builder.Field<CheckBox>("ToolVisible").Value("True").ValueChanged(OnToolVisibleChanged).Set(f => f.Text, "编辑按钮可见").Build();
        }

        private void OnToolVisibleChanged(string value)
        {
            var check = Utils.ConvertTo<bool>(value);
            toolbar?.SetItemVisible(check, "Edit");
        }
    }

    class Toolbar4 : BaseComponent
    {
        private Toolbar? toolbar;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.Component<Toolbar>()
                   .Set(c => c.Tools, Tools)
                   .Build(value => toolbar = value);

            builder.Field<CheckBox>("ToolEnabled").Value("True").ValueChanged(OnToolEnabledChanged).Set(f => f.Text, "验证、保存按钮可用").Build();
        }

        private void OnToolEnabledChanged(string value)
        {
            var check = Utils.ConvertTo<bool>(value);
            toolbar?.SetItemEnabled(check, "Check", "Save");
        }
    }
}