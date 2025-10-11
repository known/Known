namespace Known.Blazor;

/// <summary>
/// 动态表单模型配置类。
/// </summary>
/// <param name="page">表单关联的页面组件。</param>
public class DynamicFormModel(IBaseComponent page) : FormModel<Dictionary<string, object>>(page, false)
{
}