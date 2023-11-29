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
        UI = form.Page.UI;
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
            if (Column.IsFile || Column.IsMultiFile)
            {
                _inputTemplate = builder =>
                {
                    builder.Component<UploadField<TItem>>().Set(c => c.Model, this).Build();
                };
            }

            return _inputTemplate ??= builder =>
            {
                var inputType = UI.GetInputType(Column);
                builder.OpenComponent(0, inputType);
                builder.AddMultipleAttributes(1, InputAttributes);
                builder.CloseComponent();
            };
        }
    }

    //public RenderFragment ViewTemplate
    //{
    //    get { return _viewTemplate ??= builder => builder.Span($"{Value}"); }
    //}

    private IDictionary<string, object> InputAttributes
    {
        get
        {
            var expression = InputExpression.Create(this);
            var attributes = new Dictionary<string, object>
            {
                { "id", Column.Id },
                { "Value", Value },
                { "ValueExpression", expression.ValueExpression },
                { "autofocus", true },
                { "placeholder", Column.Placeholder },
            };

            if (Form.IsView || Column.IsReadOnly)
            {
                attributes["disabled"] = true;
                attributes["readonly"] = true;
            }
            else
            {
                attributes["required"] = Column.Property.IsRequired();
                attributes["ValueChanged"] = expression.ValueChanged;
            }

            UI.AddInputAttributes(attributes, Column);
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