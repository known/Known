namespace Known.Components;

/// <summary>
/// 表单字段组件类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class KField<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得或设置表单字段模型配置对象。
    /// </summary>
    [Parameter] public FieldModel<TItem> Model { get; set; }

    /// <summary>
    /// 呈现表单字段组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model.Column.Template != null)
        {
            builder.Fragment(Model.Column.Template);
        }
        else if (Model.Column.Type == FieldType.Custom)
        {
            builder.Component<CustomField<TItem>>().Set(c => c.Model, Model).Build();
        }
        else if (Model.Column.Type == FieldType.File)
        {
            builder.Component<KUploadField<TItem>>().Set(c => c.Model, Model).Build();
        }
        else
        {
            var dataType = Model.GetPropertyType();
            var inputType = UI.GetInputType(dataType, Model.Column.Type);
            if (inputType != null)
            {
                builder.OpenComponent(0, inputType);
                builder.AddMultipleAttributes(1, Model.InputAttributes);
                builder.CloseComponent();
            }
        }
    }
}

class CustomField<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public FieldModel<TItem> Model { get; set; }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!Config.FieldTypes.TryGetValue(Model.Column.CustomField, out Type type))
            return;

        var parameters = new Dictionary<string, object>();
        parameters[nameof(ICustomField.ReadOnly)] = Model.Form.IsView;
        parameters[nameof(ICustomField.Value)] = Model.Value;
        parameters[nameof(ICustomField.ValueChanged)] = delegate (object value) { Model.Value = value; };
        parameters[nameof(ICustomField.Column)] = Model.Column;
        builder.Component(type, parameters);
    }
}