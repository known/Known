namespace Known.Internals;

[NavPlugin("连接", "link")]
class NavLink : PluginBase<LinkInfo>
{
    public override void Config(Func<object, Task<Result>> onConfig)
    {
        var model = new FormModel<LinkInfo>(Parent, true)
        {
            Title = "添加连接",
            Data = new LinkInfo(),
            OnSave = d => onConfig?.Invoke(d)
        };
        Parent.UI.ShowForm(model);
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        Parameter = Utils.FromJson<LinkInfo>(Plugin?.Setting);
        if (Parameter == null)
            return;

        if (Parameter.Target == LinkTarget.Blank.ToString())
        {
            builder.Link().Href(Parameter.Url).Set("target", "_blank")
                   .Child(() =>
                   {
                       builder.Component<KIcon>()
                              .Set(c => c.Title, Parameter.Title)
                              .Set(c => c.Icon, Parameter.Icon)
                              .Build();
                   });
        }
        else
        {
            builder.Component<KIcon>()
                   .Set(c => c.Title, Parameter.Title)
                   .Set(c => c.Icon, Parameter.Icon)
                   .Set(c => c.OnClick, Parent.Callback<MouseEventArgs>(e => OnClick(Parameter)))
                   .Build();
        }
    }

    private void OnClick(LinkInfo param)
    {
        var menu = new MenuInfo
        {
            Icon = param.Icon,
            Name = param.Title,
            Url = param.Url
        };
        if (param.Target == LinkTarget.IFrame.ToString())
            menu.Target = ModuleType.IFrame.ToString();
        Parent.Navigation.NavigateTo(menu);
    }
}