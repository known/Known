namespace Known.Internals;

[NavPlugin("连接", "link")]
class NavLink : PluginBase, IPlugin
{
    public void Config(Func<object, Task<Result>> onConfig)
    {
        var model = new FormModel<LinkInfo>(Parent, true);
        model.Title = "添加连接";
        model.Data = new LinkInfo();
        model.OnSave = d => onConfig?.Invoke(d);
        Parent.UI.ShowForm(model);
    }

    public void Render(RenderTreeBuilder builder, PluginMenuInfo info)
    {
        var param = Utils.FromJson<LinkInfo>(info.Parameter);
        if (param == null)
            return;

        if (param.Target == LinkTarget.Blank.ToString())
        {
            builder.Link().Href(param.Url).Set("target", "_blank")
                   .Child(() =>
                   {
                       builder.Component<KIcon>()
                              .Set(c => c.Title, param.Title)
                              .Set(c => c.Icon, param.Icon)
                              .Build();
                   });
        }
        else
        {
            builder.Component<KIcon>()
                   .Set(c => c.Title, param.Title)
                   .Set(c => c.Icon, param.Icon)
                   .Set(c => c.OnClick, Parent.Callback<MouseEventArgs>(e => OnClick(param)))
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