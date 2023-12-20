using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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

    [DisplayName(Language.CreateBy)]
    public string CreateBy { get; set; }

    [DisplayName(Language.CreateTime)]
    public DateTime CreateTime { get; set; }

    [DisplayName(Language.ModifyBy)]
    public string ModifyBy { get; set; }

    [DisplayName(Language.ModifyTime)]
    public DateTime? ModifyTime { get; set; }

    public int Version { get; set; }

    public string Extension { get; set; }

    [Required(ErrorMessage = "AppId不能为空！")]
    public string AppId { get; set; }

    [Required(ErrorMessage = $"{Language.CompNo}不能为空！")]
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
    //    var properties = GetType().GetProperties();
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

    public virtual Result Validate()
    {
        var type = GetType();
        var properties = type.GetProperties();
        var dicError = new Dictionary<string, List<string>>();

        foreach (var pi in properties)
        {
            var attrs = pi.GetCustomAttributes(true);
            var value = pi.GetValue(this, null);
            var valueString = value == null ? "" : value.ToString().Trim();
            var errors = new List<string>();
            foreach (var item in attrs)
            {
                if (item is ColumnAttribute column)
                    column.Validate(value, pi, errors);
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