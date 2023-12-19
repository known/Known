using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class FieldModel<TItem> where TItem : class, new()
{
    private readonly IUIService UI;

    internal FieldModel(FormModel<TItem> form, ColumnInfo column)
    {
        if (!string.IsNullOrWhiteSpace(column.Id))
            form.Fields[column.Id] = this;
        UI = form.UI;
        Form = form;
        Column = column;
        Data = form.Data;
    }

    public FormModel<TItem> Form { get; }
    public ColumnInfo Column { get; }
    internal TItem Data { get; }
    internal Action OnStateChanged { get; set; }

    public object Value
    {
        get => Column.Property?.GetValue(Form.Data);
        set
        {
            if (Column.Property?.SetMethod is not null && !Equals(Value, value))
            {
                Column.Property?.SetValue(Form.Data, value);
                Form.OnFieldChanged?.Invoke(Column.Id);
            }
        }
    }

    public void StateChanged() => OnStateChanged?.Invoke();

    public List<CodeInfo> GetCodes(string emptyText = "请选择")
    {
        var codes = Form.GetCodes(Column);
        if (codes == null || codes.Count == 0)
            codes = Cache.GetCodes(Column.Category);

        return codes.ToCodes(emptyText);
    }

    private bool IsReadOnly => Form.IsView || Column.IsReadOnly;

    internal IDictionary<string, object> InputAttributes
    {
        get
        {
            var attributes = new Dictionary<string, object>
            {
                { "id", Column.Id },
                { "autofocus", true },
                { "disabled", IsReadOnly },
                { "readonly", IsReadOnly },
                { "required", Column.IsRequired },
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
                return args.Length == 2 &&
                       args[0].ParameterType == typeof(object) &&
                       args[1].ParameterType.IsGenericType &&
                       args[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>);
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