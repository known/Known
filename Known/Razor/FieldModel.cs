using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class FieldModel<TItem> where TItem : class, new()
{
    private readonly IUIService UI;
    private FormModel<TItem> _form;
    private RenderFragment _inputTemplate;

    internal FieldModel(FormModel<TItem> form, ColumnAttribute column)
    {
        UI = form.Page.UI;
        _form = form;
        Column = column;
        Data = form.Data;
    }

    public ColumnAttribute Column { get; }
    public TItem Data { get; }
    public Action<TItem, object> ValueChanged { get; set; }

    public object Value
    {
        get => Column.Property.GetValue(_form.Data);
        set
        {
            if (Column.Property.SetMethod is not null && !Equals(Value, value))
            {
                Column.Property.SetValue(_form.Data, value);
                ValueChanged?.Invoke(_form.Data, value);
            }
        }
    }

    public RenderFragment InputTemplate
    {
        get
        {
            return _inputTemplate ??= builder =>
            {
                var inputType = UI.GetInputType(Column);
                builder.OpenComponent(0, inputType);
                builder.AddMultipleAttributes(1, InputAttributes);
                builder.CloseComponent();
            };
        }
    }

    private IDictionary<string, object> InputAttributes
    {
        get
        {
            var expression = InputExpression.Create(this);
            var attributes = new Dictionary<string, object>
            {
                { "id", Column.Property.Name },
                { "Value", Value },
                { "ValueExpression", expression.ValueExpression },
                { "autofocus", true },
                { "required", Column.Property.IsRequired() },
                { "placeholder", Column.Placeholder },
            };
            if (_form.IsView)
            {
                attributes["disabled"] = true;
                attributes["readonly"] = true;
            }
            else
            {
                attributes["ValueChanged"] = expression.ValueChanged;
            }
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
        // () => Owner.Property
        var property = model.Column.Property;
        var access = Expression.Property(
            Expression.Constant(model.Data, typeof(TItem)),
            property);
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