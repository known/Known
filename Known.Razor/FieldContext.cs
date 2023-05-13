namespace Known.Razor;

public class FieldContext
{
    internal FieldContext()
    {
        Fields = new Dictionary<string, Field>();
    }

    internal bool ReadOnly { get; set; }
    internal bool IsTableForm { get; set; }
    internal string CheckFields { get; set; }
    public string FieldId { get; set; }
    internal object FieldValue { get; set; }
    public Dictionary<string, Field> Fields { get; }
    internal CodeInfo[] FieldItems { get; set; }
    internal Dictionary<string, object> DicModel { get; private set; }

    private object model;
    internal object Model
    {
        get { return model; }
        set
        {
            model = value;
            DicModel = Utils.MapTo<Dictionary<string, object>>(value);
        }
    }

    internal void SetModel(string name, object value)
    {
        if (Model == null || string.IsNullOrWhiteSpace(name))
            return;

        TypeHelper.SetValue(Model, name, value);
    }

    public T FieldAs<T>(string id) where T : Field => Fields[id] as T;
    public T FieldValueAs<T>() => Utils.ConvertTo<T>(FieldValue);
}