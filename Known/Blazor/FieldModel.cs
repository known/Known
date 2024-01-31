using System.Linq.Expressions;
using System.Reflection;
using Known.Extensions;
using Microsoft.AspNetCore.Components;

namespace Known.Blazor;

public class FieldModel<TItem> : BaseModel where TItem : class, new()
{
    internal FieldModel(FormModel<TItem> form, ColumnInfo column) : base(form.Context)
    {
        if (!string.IsNullOrWhiteSpace(column.Id))
            form.Fields[column.Id] = this;
        Form = form;
        Column = column;
        Data = form.Data;
        IsDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
    }

    public FormModel<TItem> Form { get; }
    public ColumnInfo Column { get; }
    public bool IsReadOnly => Form.IsView || Column.ReadOnly;
    internal bool IsDictionary { get; }
    internal TItem Data { get; }
    internal PropertyInfo Property => Column.Property;
    internal Action OnStateChanged { get; set; }

    public Type GetPropertyType()
    {
        if (IsDictionary)
        {
            var value = Value;
            switch (Column.Type)
            {
                case FieldType.Date:
                    return value != null ? value.GetType() : typeof(DateTime?);
                case FieldType.Number:
                    return value != null ? value.GetType() : typeof(int);
                case FieldType.Switch:
                case FieldType.CheckBox:
                    return typeof(bool);
                case FieldType.CheckList:
                    return typeof(string[]);
                default:
                    return typeof(string);
            }
        }

        return Property?.PropertyType;
    }

    public object Value
    {
        get
        {
            if (IsDictionary)
            {
                var data = Form.Data as Dictionary<string, object>;
                data.TryGetValue(Column.Id, out object value);
                if (Column.Type != FieldType.Date)
                    return value;

                return Utils.ConvertTo<DateTime?>(value);
            }

            return Column.Property?.GetValue(Form.Data);
        }
        set
        {
            if (!Equals(Value, value))
            {
                if (IsDictionary)
                    (Form.Data as Dictionary<string, object>)[Column.Id] = value;
                else if (Column.Property?.SetMethod is not null)
                    Column.Property?.SetValue(Form.Data, value);
                Form.OnFieldChanged?.Invoke(Column.Id);
            }
        }
    }

    public void StateChanged() => OnStateChanged?.Invoke();

    public List<CodeInfo> GetCodes(string emptyText = "Please select")
    {
        if (!string.IsNullOrWhiteSpace(emptyText))
            emptyText = Language?["PleaseSelect"];

        var codes = Form.GetCodes(Column);
        if (codes != null)
            return codes.ToCodes(emptyText);

        codes = Cache.GetCodes(Column.Category);
        foreach (var item in codes)
        {
            var name = Language?.GetString(item);
            if (!string.IsNullOrWhiteSpace(name))
                item.Name = name;
        }
        return codes.ToCodes(emptyText);
    }

    internal IDictionary<string, object> InputAttributes
    {
        get
        {
            var attributes = new Dictionary<string, object>
            {
                { "id", Column.Id },
                { "autofocus", true },
                { "readonly", IsReadOnly },
                { "required", Column.Required },
                { "placeholder", Column.Placeholder }
            };
            if (Column.Type != FieldType.Date)
                attributes["disabled"] = IsReadOnly;
            UI.AddInputAttributes(attributes, this);

            var expression = InputExpression.Create(this);
            attributes["Value"] = Value;
            attributes["ValueChanged"] = expression?.ValueChanged;
            if (expression?.ValueExpression != null)
                attributes["ValueExpression"] = expression?.ValueExpression;

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
        var propertyType = model.GetPropertyType();
        if (propertyType == null)
            return null;

        //TODO：动态数据Value表达式
        LambdaExpression lambda = null;
        if (model.Property != null)
        {
            var access = Expression.Property(Expression.Constant(model.Data, typeof(TItem)), model.Property);
            lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(propertyType), access);
        }

        // Create(object receiver, Action<object>) callback
        var method = EventCallbackFactory.MakeGenericMethod(propertyType);

        // value => Field.Value = value;
        var changeHandlerParameter = Expression.Parameter(propertyType);

        var body = Expression.Assign(
            Expression.Property(Expression.Constant(model), nameof(model.Value)),
            Expression.Convert(changeHandlerParameter, typeof(object)));

        var changeHandlerLambda = Expression.Lambda(
            typeof(Action<>).MakeGenericType(propertyType),
            body,
            changeHandlerParameter);

        var changeHandler = method.Invoke(
            EventCallback.Factory,
            new object[] { model, changeHandlerLambda.Compile() });

        return new InputExpression(lambda, changeHandler);
    }
}