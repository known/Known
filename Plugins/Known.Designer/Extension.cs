using Known.Designer.Entity;
using Known.Designer.Flows;
using Known.Designer.Forms;
using Known.Designer.Pages;

namespace Known.Designer;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加Known无代码设计器和代码生成器。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">选项委托。</param>
    public static void AddKnownDesigner(this IServiceCollection services, Action<DesignerOption> action = null)
    {
        var assembly = typeof(Extension).Assembly;
        Config.AddModule(assembly);
        UIConfig.ModuleFormTabs["ModelSetting"] = BuildModuleModel;
        //UIConfig.ModuleFormTabs["FlowSetting"] = BuildModuleFlow;
        UIConfig.ModuleFormTabs["PageSetting"] = BuildModulePage;
        UIConfig.ModuleFormTabs["FormSetting"] = BuildModuleForm;
        KStyleSheet.AddStyle("_content/Known.Designer/css/web.css");

        services.AddSingleton<ICodeGenerator, CodeGenerator>();
    }

    private static void BuildModuleModel(RenderTreeBuilder builder, ModuleInfo model)
    {
        builder.Component<EntityDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               .Set(c => c.Model, model.EntityData)
               .Set(c => c.OnChanged, data => model.EntityData = data)
               .Build();
    }

    private static void BuildModuleFlow(RenderTreeBuilder builder, ModuleInfo model)
    {
        builder.Component<FlowDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               .Set(c => c.Model, model.FlowData)
               .Set(c => c.OnChanged, data => model.FlowData = data)
               .Build();
    }

    private static void BuildModulePage(RenderTreeBuilder builder, ModuleInfo model)
    {
        model.Entity.PageUrl = model.Url;
        builder.Component<PageDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               .Set(c => c.Model, model.Page)
               .Set(c => c.OnChanged, data => model.Page = data)
               .Build();
    }

    private static void BuildModuleForm(RenderTreeBuilder builder, ModuleInfo model)
    {
        builder.Component<FormDesigner>()
               .Set(c => c.ReadOnly, model.IsView)
               .Set(c => c.Module, model)
               //.Set(c => c.Flow, Flow)
               .Set(c => c.Model, model.Form)
               .Set(c => c.OnChanged, data => model.Form = data)
               .Build();
    }
}