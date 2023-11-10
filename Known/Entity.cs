using System.Dynamic;

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

    [Column("ID", "", false, "1", "50", IsForm = true)]
    public string Id { get; set; }

    [Column(Language.CreateBy, "", true, "1", "50", IsGrid = true)]
    public string CreateBy { get; set; }

    [Column(Language.CreateTime, "", true, IsGrid = true)]
    public DateTime CreateTime { get; set; }

    [Column(Language.ModifyBy, "", false, "1", "50", IsGrid = true)]
    public string ModifyBy { get; set; }

    [Column(Language.ModifyTime, "", false, IsGrid = true)]
    public DateTime? ModifyTime { get; set; }

    [Column(Language.Version, "", true)]
    public int Version { get; set; }

    [Column("Extension")]
    public string Extension { get; set; }

    [Column("AppId", "", true, "1", "50")]
    public string AppId { get; set; }

    [Column(Language.CompNo, "", true, "1", "50")]
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

    public void FillModel(ExpandoObject model)
    {
        var properties = GetType().GetProperties();
        foreach (var pi in model)
        {
            var name = pi.Key;
            if (name == "Id")
                continue;

            var value = pi.Value;
            var property = properties.FirstOrDefault(p => p.Name == name);
            if (property != null)
            {
                value = Utils.ConvertTo(property.PropertyType, value);
                property.SetValue(this, value);
            }
        }
    }

    public virtual Result Validate()
    {
        var type = GetType();
        var properties = type.GetProperties();
        var dicError = new Dictionary<string, List<string>>();

        foreach (var pi in properties)
        {
            var attrs = pi.GetCustomAttributes(true);
            var value = pi.GetValue(this, null);
            var errors = new List<string>();
            foreach (var item in attrs)
            {
                if (item is ColumnAttribute column)
                    column.Validate(value, pi.PropertyType, errors);
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
                item.ForEach(m => result.AddError(m));
            }
            return result;
        }

        return Result.Success("");
    }
}