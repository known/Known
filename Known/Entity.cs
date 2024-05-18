namespace Known;

public class EntityBase
{
    private Dictionary<string, object> original;

    public EntityBase()
    {
        IsNew = true;
        Id = Utils.GetGuid();
        CreateBy = "temp";
        CreateTime = DateTime.Now;
        Version = 1;
        AppId = "temp";
        CompNo = "temp";
    }

    public virtual bool IsNew { get; internal set; }

    public string Id { get; set; }
    public string CreateBy { get; set; }
    public DateTime CreateTime { get; set; }
    public string ModifyBy { get; set; }
    public DateTime? ModifyTime { get; set; }
    public int Version { get; set; }
    public string Extension { get; set; }
    public string AppId { get; set; }
    public string CompNo { get; set; }

    internal void SetOriginal(Dictionary<string, object> original)
    {
        IsNew = false;
        this.original = original;
    }

    internal bool IsChanged(string propertyName, object value)
    {
        if (original == null || !original.ContainsKey(propertyName))
            return true;

        var orgValue = original[propertyName];
        if (orgValue == null)
            return true;

        return !orgValue.Equals(value);
    }

    //public void FillModel(ExpandoObject model)
    //{
    //    var properties = TypeHelper.Properties(GetType());
    //    foreach (var pi in model)
    //    {
    //        var name = pi.Key;
    //        if (name == "Id")
    //            continue;

    //        var value = pi.Value;
    //        var property = properties.FirstOrDefault(p => p.Name == name);
    //        if (property != null)
    //        {
    //            value = Utils.ConvertTo(property.PropertyType, value);
    //            property.SetValue(this, value);
    //        }
    //    }
    //}

    public virtual Result Validate(Context context)
    {
        var type = GetType();
        var properties = TypeHelper.Properties(type);
        var dicError = new Dictionary<string, List<string>>();

        foreach (var pi in properties)
        {
            var attrs = pi.GetCustomAttributes(true);
            var value = pi.GetValue(this, null);
            var valueString = value == null ? "" : value.ToString().Trim();
            var errors = new List<string>();
            var column = new ColumnAttribute();
            column.Validate(context, value, pi, errors);
            foreach (var item in attrs)
            {
                if (item is RegexAttribute regex)
                    regex.Validate(value, errors);
            }
            if (errors.Count > 0)
                dicError.Add(pi.Name, errors);
        }

        if (dicError.Count > 0)
        {
            var result = Result.Error("", dicError);
            foreach (var item in dicError.Values)
            {
                item.ForEach(result.AddError);
            }
            return result;
        }

        return Result.Success("");
    }
}