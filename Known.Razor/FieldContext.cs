namespace Known.Razor;

public class FieldContext
{
    internal FieldContext()
    {
        Fields = new Dictionary<string, Field>();
    }

    internal bool ReadOnly { get; set; }
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

class TableContext { }

class QueryContext : FieldContext
{
    internal List<QueryInfo> GetData()
    {
        var infos = new List<QueryInfo>();
        foreach (var item in Fields)
        {
            var value = item.Value.Value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var query = new QueryInfo(item.Key, value);
                infos.Add(query);
            }
        }
        return infos;
    }

    internal static List<QueryInfo> GetData(object query)
    {
        var dics = Utils.MapTo<Dictionary<string, string>>(query);
        if (dics == null || dics.Count == 0)
            return null;

        var infos = new List<QueryInfo>();
        foreach (var item in dics)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
                infos.Add(new QueryInfo(item.Key, item.Value));
        }
        return infos;
    }
}