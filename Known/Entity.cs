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

    public virtual bool IsNew { get; set; }

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

    public virtual Result Validate(Context context)
    {
        var type = GetType();
        var properties = TypeHelper.Properties(type);
        var dicError = new Dictionary<string, List<string>>();

        foreach (var pi in properties)
        {
            var value = pi.GetValue(this, null);
            var errors = new List<string>();
            pi.Validate(context.Language, value, errors);
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