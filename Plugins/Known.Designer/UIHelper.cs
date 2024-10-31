using Known.Designer.Entity;
using Known.Designer.Flows;
using Known.Designer.Forms;
using Known.Designer.Pages;

namespace Known.Designer;

class UIHelper
{
    internal static void BuildModuleForm(TabModel tab, FormModel<SysModule> model)
    {
        tab.AddTab("ModelSetting", b => BuildModuleModel(b, model));
        //tab.AddTab("FlowSetting", b => BuildModuleFlow(b, model));
        tab.AddTab("PageSetting", b => BuildModulePage(b, model));
        tab.AddTab("FormSetting", b => BuildModuleForm(b, model));
    }

    private static void BuildModuleModel(RenderTreeBuilder builder, FormModel<SysModule> model)
    {
        builder.Component<EntityDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model.Data)
               .Set(c => c.Model, model.Data.EntityData)
               .Set(c => c.OnChanged, data => model.Data.EntityData = data)
               .Build();
    }

    private static void BuildModuleFlow(RenderTreeBuilder builder, FormModel<SysModule> model)
    {
        builder.Component<FlowDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model.Data)
               .Set(c => c.Model, model.Data.FlowData)
               .Set(c => c.OnChanged, data => model.Data.FlowData = data)
               .Build();
    }

    private static void BuildModulePage(RenderTreeBuilder builder, FormModel<SysModule> model)
    {
        model.Data.Entity.PageUrl = model.Data.Url;
        builder.Component<PageDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model.Data)
               .Set(c => c.Model, model.Data.Page)
               .Set(c => c.OnChanged, data => model.Data.Page = data)
               .Build();
    }

    private static void BuildModuleForm(RenderTreeBuilder builder, FormModel<SysModule> model)
    {
        builder.Component<FormDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model.Data)
               //.Set(c => c.Flow, Flow)
               .Set(c => c.Model, model.Data.Form)
               .Set(c => c.OnChanged, data => model.Data.Form = data)
               .Build();
    }
}