using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class FieldModel<TItem> where TItem : class, new()
{
    private readonly IUIService UI;
    private RenderFragment _inputTemplate;
    //private RenderFragment _viewTemplate;

    internal FieldModel(FormModel<TItem> form, ColumnInfo column)
    {
        UI = form.UI;
        Form = form;
        Column = column;
        Data = form.Data;
    }

    public FormModel<TItem> Form { get; }
    public ColumnInfo Column { get; }
    public TItem Data { get; }
    public Action<TItem, object> ValueChanged { get; set; }

    public object Value
    {
        get => Column.Property.GetValue(Form.Data);
        set
        {
            if (Column.Property.SetMethod is not null && !Equals(Value, value))
            {
                Column.Property.SetValue(Form.Data, value);
                ValueChanged?.Invoke(Form.Data, value);
            }
        }
    }

    public RenderFragment InputTemplate
    {
        get
        {
            if (Column.Template != null)
            {
                _inputTemplate = Column.Template;
            }
            else if (Column.IsFile || Column.IsMultiFile)
            {
                _inputTemplate = builder => builder.Component<UploadField<TItem>>().Set(c => c.Model, this).Build();
            }
            else
            {
                _inputTemplate = builder =>
                {
                    var inputType = UI.GetInputType(Column);
                    if (inputType != null)
                    {
                        builder.OpenComponent(0, inputType);
                        builder.AddMultipleAttributes(1, InputAttributes);
                        builder.CloseComponent();
                    }
                };
            }

            return _inputTemplate;
        }
    }

    //public RenderFragment ViewTemplate
    //{
    //    get { return _viewTemplate ??= builder => builder.Span($"{Value}"); }
    //}

    public List<CodeInfo> GetCodes(string emptyText = "请选择")
    {
        var codes = Form.GetCodes(Column);
        if (codes == null || codes.Count == 0)
            codes = Cache.GetCodes(Column.Category);

        return codes.ToCodes(emptyText);
    }

    private bool IsReadOnly => Form.IsView || Column.IsReadOnly;

    private IDictionary<string, object> InputAttributes
    {
        get
        {
            var attributes = new Dictionary<string, object>
            {
                { "id", Column.Id },
                { "autofocus", true },
                { "disabled", IsReadOnly },
                { "readonly", IsReadOnly },
                { "required", Column.Property.IsRequired() },
                { "placeholder", Column.Placeholder }
            };
            UI.AddInputAttributes(attributes, this);

            var expression = InputExpression.Create(this);
            attributes["Value"] = Value;
            attributes["ValueChanged"] = expression.ValueChanged;
            attributes["ValueExpression"] = expression.ValueExpression;
            return attributes;
        }
    }
}

record InputExpression(LambdaExpression ValueExpression, object ValueChanged)
{
    private static readonly MethodInfo EventCallbackFactory = GetEventCallbackFactory();

    private static MethodInfo GetEventCallbackFactory()
    {
        return typeof(EventCallbackFactory)
            .GetMethods()
            .Single(m =>
            {
                if (m.Name != "Create" || !m.IsPublic || m.IsStatic || !m.IsGenericMethod)
                    return false;

                var generic = m.GetGenericArguments();
                if (generic.Length != 1)
                    return false;

                var args = m.GetParameters();
                return args.Length == 2
                       && args[0].ParameterType == typeof(object)
                       && args[1].ParameterType.IsGenericType
                       && args[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>);
            });
    }

    public static InputExpression Create<TItem>(FieldModel<TItem> model) where TItem : class, new()
    {
        var property = model.Column.Property;
        var access = Expression.Property(Expression.Constant(model.Data, typeof(TItem)), property);
        var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(property.PropertyType), access);

        // Create(object receiver, Action<object>) callback
        var method = EventCallbackFactory.MakeGenericMethod(property.PropertyType);

        // value => Field.Value = value;
        var changeHandlerParameter = Expression.Parameter(property.PropertyType);

        var body = Expression.Assign(
            Expression.Property(Expression.Constant(model), nameof(model.Value)),
            Expression.Convert(changeHandlerParameter, typeof(object)));

        var changeHandlerLambda = Expression.Lambda(
            typeof(Action<>).MakeGenericType(property.PropertyType),
            body,
            changeHandlerParameter);

        var changeHandler = method.Invoke(
            EventCallback.Factory,
            new object[] { model, changeHandlerLambda.Compile() });

        return new InputExpression(lambda, changeHandler);
    }
}